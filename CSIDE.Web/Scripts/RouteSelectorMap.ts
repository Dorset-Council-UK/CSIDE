import { Map, View } from "ol";
import Feature from "ol/Feature";
import { extend } from "ol/extent";
import { GeoJSON } from "ol/format";
import { MultiLineString, Point } from "ol/geom";
import { Draw } from "ol/interaction";
import { Tile as TileLayer, Vector as VectorLayer } from "ol/layer";
import { register } from "ol/proj/proj4";
import { TileWMS } from "ol/source";
import VectorSource from "ol/source/Vector";
import XYZ from "ol/source/XYZ";
import { Stroke, Style } from "ol/style";
import { TileGrid } from "ol/tilegrid";
import proj4 from "proj4";

const geomStyle = new Style({
  stroke: new Stroke({
    color: '#00ffff',
    width: 4,
    lineDash: [4,6]
  })
});

const highlighterStyle = new Style({
  stroke: new Stroke({
    color: 'rgba(255,255,0,0.5)',
    width: 10,
  })
});


let map: Map;
let geometrySource: VectorSource;
let existingRoutesSource: VectorSource;
const bngTilegrid: TileGrid = new TileGrid({
  "resolutions": [896.0, 448.0, 224.0, 112.0, 56.0, 28.0, 14.0, 7.0, 3.5, 1.75, 0.875, 0.4375, 0.21875, 0.109375],
  "origin": [-238375.0, 1376256.0]
});

export function initMap(geometry: string, existingRoutes: string, mapConfigJSON: string, component: any) {

  setProj4Defs();

  const mapConfig = JSON.parse(mapConfigJSON);

  const basemap = new XYZ(
    {
      url: `https://api.os.uk/maps/raster/v1/zxy/${mapConfig.basemap}_27700/{z}/{x}/{y}.png?key=${mapConfig.osMapsAPIKey}`,
      attributions: `&copy; Crown copyright and database rights ${new Date().getFullYear()} <a href="https://os.uk" target="_blank">OS</a> ${mapConfig.osLicenceNumber}.`,
      attributionsCollapsible: false,
      tileGrid: bngTilegrid,
      projection: 'EPSG:27700'
    });

  map = new Map({
    target: mapConfig.mapId,
    layers: [
      new TileLayer({
        source: basemap
      })
    ],
    view: new View({
      projection: 'EPSG:27700'
    })
  });

  map.getView().fit(mapConfig.bounds, { size: map.getSize() });

  if (mapConfig.overlays) {
    mapConfig.overlays.forEach((overlay: { mapServiceBaseURL: any; layerName: any; }) => {
      const overlaySource = new TileWMS({
        url: overlay.mapServiceBaseURL,
        params: { 'LAYERS': overlay.layerName, 'v': crypto.randomUUID() },
      });
      const layer = new TileLayer({ source: overlaySource });
      map.addLayer(layer);
    });
  }
  initGeometry(geometry);
  initExistingRoutes(existingRoutes);
  initEditing(component);


}

function initEditing(component: any) {
  
  const pickRoute = new PickRoute();
  map.addInteraction(pickRoute);
  pickRoute.on('drawend', async (e) => {
    await component.invokeMethodAsync('AddOrRemoveRoute', (e.feature.getGeometry() as Point).getCoordinates());
    
  });
  map.getViewport().style.cursor = 'crosshair';
      
}

function initGeometry(geometry: string) {
  geometrySource = new VectorSource();
  if (geometry) {
    const features = new GeoJSON().readFeatures(geometry);
    if (features) {
      const mapExtent = features[0].getGeometry()!.getExtent();
      features.forEach(feature => {
        extend(mapExtent, feature.getGeometry()!.getExtent())
      });
      map.getView().fit(mapExtent, {
        padding: [20, 20, 20, 20],
        maxZoom: 16
      });

      // Split MultiLineString into individual LineString features
      features.forEach(feature => {
        const geom = feature.getGeometry();
        if (geom) {
          if (geom.getType() === 'MultiLineString') {
            const lineStrings = (geom as MultiLineString).getLineStrings();
            lineStrings.forEach(lineString => {
              const lineFeature = new Feature({
                geometry: lineString
              });
              geometrySource.addFeature(lineFeature);
            });
          } else {
            geometrySource.addFeature(feature);
          }
        }
      });

    }
    let geometryLayer = new VectorLayer({ source: geometrySource, style: geomStyle });
    map.addLayer(geometryLayer);
  }
}

function initExistingRoutes(existingRoutes: string) {
  existingRoutesSource = new VectorSource();
  if (existingRoutes) {
    const featureCollection = {
      type: "FeatureCollection",
      features: JSON.parse(existingRoutes)
    };
    const features = new GeoJSON().readFeatures(featureCollection);
    existingRoutesSource.addFeatures(features);

    let geometryLayer = new VectorLayer({ source: existingRoutesSource, style: highlighterStyle });
    map.addLayer(geometryLayer);

  }
}

function setProj4Defs() {
//convert x/y from BNG to Spherical Mercator
  proj4.defs(
    'EPSG:27700',
    '+proj=tmerc +lat_0=49 +lon_0=-2 +k=0.9996012717 ' +
    '+x_0=400000 +y_0=-100000 +ellps=airy ' +
    '+towgs84=446.448,-125.157,542.06,0.15,0.247,0.842,-20.489 ' +
    '+units=m +no_defs',
  );
  register(proj4);
}

/**Custom interaction that slightly abuses the OpenLayers 'Draw' control to allow arbitrary selection of routes*/
class PickRoute extends Draw {
  constructor() {
    super({
      type: 'Point',
      style: {}
    });
  }
}

export function updateExistingRoutes(existingRoutes: string) {
  clearDrawnGeometries();
  if (existingRoutes) {
    const featureCollection = {
      type: "FeatureCollection",
      features: JSON.parse(existingRoutes)
    };
    const features = new GeoJSON().readFeatures(featureCollection);
    existingRoutesSource.addFeatures(features);
  }
}
export function clearDrawnGeometries() {
  existingRoutesSource.clear();
};
