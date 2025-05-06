import { Map, View } from "ol";
import { extend } from "ol/extent";
import { GeoJSON } from "ol/format";
import { Tile as TileLayer, Vector as VectorLayer } from "ol/layer";
import { register } from "ol/proj/proj4";
import { TileWMS } from "ol/source";
import VectorSource from "ol/source/Vector";
import XYZ from "ol/source/XYZ";
import { Fill, Icon, Stroke, Style } from "ol/style";
import { TileGrid } from "ol/tilegrid";
import proj4 from "proj4";

// Global class to manage all maps
export class MiniMapManager {
  private static instance: MiniMapManager;
  private maps: { [mapId: string]: MiniMap } = {};

  private constructor() {
    // Private constructor to enforce singleton pattern
  }

  public static getInstance(): MiniMapManager {
    if (!MiniMapManager.instance) {
      MiniMapManager.instance = new MiniMapManager();
    }
    return MiniMapManager.instance;
  }

  public initMap(geometry: string, geomSRID: number, mapConfigJSON: string): void {
    const mapConfig = JSON.parse(mapConfigJSON);
    const mapId = mapConfig.mapId;

    // Create a new MiniMap instance if it doesn't exist
    if (!this.maps[mapId]) {
      this.maps[mapId] = new MiniMap(mapConfig);
    }

    // Initialize the map with the provided data
    this.maps[mapId].init(geometry, geomSRID);
  }

  public updateMap(geometry: string, geomSRID: number, z: number, mapId: string): void {
    const map = this.maps[mapId];
    if (!map) {
      console.error(`Map with id ${mapId} not found`);
      return;
    }
    map.update(geometry, geomSRID, z);
  }

  public getMap(mapId: string): MiniMap | undefined {
    return this.maps[mapId];
  }
}

// Class to manage individual maps
class MiniMap {
  private map: Map;
  private vectorSource: VectorSource;
  private vectorLayer: VectorLayer<VectorSource>;
  private mapConfig: any;
  private readonly bngTilegrid: TileGrid;

  private iconStyle = new Style({
    image: new Icon({
      anchor: [0.5, 32],
      anchorXUnits: "fraction",
      anchorYUnits: "pixels",
      crossOrigin: 'anonymous',
      color: window.getComputedStyle(document.body).getPropertyValue('--primary-color'),
      src: `./img/map-pin.svg`,
    }),
  });

  private lineStyle = new Style({
    stroke: new Stroke({
      color: 'rgba(255,255,0,0.5)',
      width: 10,
    })
  });

  private polygonStyle = new Style({
    fill: new Fill({
      color: 'rgba(0,0,0,0.3)',
    })
  });

  constructor(mapConfig: any) {
    this.mapConfig = mapConfig;

    this.bngTilegrid = new TileGrid({
      "resolutions": [896.0, 448.0, 224.0, 112.0, 56.0, 28.0, 14.0, 7.0, 3.5, 1.75, 0.875, 0.4375, 0.21875, 0.109375],
      "origin": [-238375.0, 1376256.0]
    });

    // Setup proj4 definitions
    this.setProj4Defs();

    // Create the base map
    const basemap = new XYZ({
      url: `https://api.os.uk/maps/raster/v1/zxy/${mapConfig.basemap}_27700/{z}/{x}/{y}.png?key=${mapConfig.osMapsAPIKey}`,
      attributions: `&copy; Crown copyright and database rights ${new Date().getFullYear()} <a href="https://os.uk" target="_blank">OS</a> ${mapConfig.osLicenceNumber}.`,
      attributionsCollapsible: false,
      tileGrid: this.bngTilegrid,
      projection: 'EPSG:27700'
    });

    // Create map
    this.map = new Map({
      target: mapConfig.mapId,
      layers: [
        new TileLayer({
          source: basemap
        }),
      ],
      view: new View({
        projection: 'EPSG:27700'
      })
    });

    // Set up vector layer
    this.vectorSource = new VectorSource();
    this.vectorLayer = new VectorLayer({
      source: this.vectorSource,
      style: [this.iconStyle, this.lineStyle, this.polygonStyle]
    });

    // Add any overlay layers if specified
    if (mapConfig.overlays) {
      this.addOverlays();
    }
    // Add vector layer to map
    this.map.addLayer(this.vectorLayer);
  }

  private setProj4Defs(): void {
    proj4.defs(
      'EPSG:27700',
      '+proj=tmerc +lat_0=49 +lon_0=-2 +k=0.9996012717 ' +
      '+x_0=400000 +y_0=-100000 +ellps=airy ' +
      '+towgs84=446.448,-125.157,542.06,0.15,0.247,0.842,-20.489 ' +
      '+units=m +no_defs',
    );
    register(proj4);
  }

  private addOverlays(): void {
    this.mapConfig.overlays.forEach((overlay: { mapServiceBaseURL: any; layerName: any; }) => {
      const overlaySource = new TileWMS({
        url: overlay.mapServiceBaseURL,
        params: { 'LAYERS': overlay.layerName, 'v': crypto.randomUUID() },
      });
      const layer = new TileLayer({ source: overlaySource });
      this.map.addLayer(layer);
    });
  }

  public init(geometry: string, geomSRID: number): void {
    if (geometry) {
      this.addGeometryAndCenter(geometry, geomSRID, 18);
    }
  }

  public update(geometry: string, geomSRID: number, z: number): void {
    this.vectorSource.clear();
    if (geometry) {
      this.addGeometryAndCenter(geometry, geomSRID, z);
    }
  }

  private addGeometryAndCenter(geometry: string, geomSRID: number, maxZoom: number): void {
    const features = new GeoJSON().readFeatures(geometry);

    if (geomSRID !== 27700) {
      features?.forEach(f => {
        f.getGeometry()?.transform(`EPSG:${geomSRID}`, 'EPSG:27700');
      });
    }

    if (features && features.length > 0) {
      const mapExtent = features[0].getGeometry()!.getExtent();

      features.forEach(feature => {
        extend(mapExtent, feature.getGeometry()!.getExtent())
      });

      this.map.getView().fit(mapExtent, {
        padding: [20, 20, 20, 20],
        maxZoom: maxZoom
      });

      const newVectorSource = new VectorSource({
        features: features,
      });

      this.vectorLayer.setSource(newVectorSource);
      this.vectorSource = newVectorSource;
      this.vectorLayer.setVisible(true);
    }
  }

  // Helper methods for Blazor JS interop
  public getMap(): Map {
    return this.map;
  }

  public clearGeometry(): void {
    this.vectorSource.clear();
  }

  public setZoom(zoom: number): void {
    this.map.getView().setZoom(zoom);
  }

  public getZoom(): number {
    return this.map.getView().getZoom() || 0;
  }

  public getCenter(): number[] {
    return this.map.getView().getCenter() || [0, 0];
  }

  public setCenter(center: number[]): void {
    this.map.getView().setCenter(center);
  }
}

// Export JS interop accessible functions
export function initMap(geometry: string, geomSRID: number, mapConfigJSON: string): void {
  MiniMapManager.getInstance().initMap(geometry, geomSRID, mapConfigJSON);
}

export function updateMap(geometry: string, geomSRID: number, z: number, mapId: string): void {
  MiniMapManager.getInstance().updateMap(geometry, geomSRID, z, mapId);
}

// Additional JS interop methods
export function clearGeometry(mapId: string): void {
  const map = MiniMapManager.getInstance().getMap(mapId);
  if (map) {
    map.clearGeometry();
  }
}

export function setZoom(mapId: string, zoom: number): void {
  const map = MiniMapManager.getInstance().getMap(mapId);
  if (map) {
    map.setZoom(zoom);
  }
}

export function getZoom(mapId: string): number {
  const map = MiniMapManager.getInstance().getMap(mapId);
  return map ? map.getZoom() : 0;
}
