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

const computedStyles = window.getComputedStyle(document.body);
const themeColor = computedStyles.getPropertyValue('--primary-color'); //get the primary color from the CSS variables
const iconStyle = new Style({
  image: new Icon({
    anchor: [0.5, 32], //50% from left, 32px (height of icon) up. Pins the tail of the icon to the right location, rather than the center
    anchorXUnits: "fraction",
    anchorYUnits: "pixels",
    crossOrigin: 'anonymous',
    color: themeColor,
    src: `./img/map-pin.svg`, //TODO this won't work in contexts where the app isn't running on root
  }),
});
const lineStyle = new Style({
  stroke: new Stroke({
    color: 'rgba(255,255,0,0.5)',
    width: 10,
  })
});
const polygonStyle = new Style({
  fill: new Fill({
    color: 'rgba(0,0,0,0.3)',
  })
});

let map: Map;
let vectorSource: VectorSource;
const bngTilegrid: TileGrid = new TileGrid({
  "resolutions": [896.0, 448.0, 224.0, 112.0, 56.0, 28.0, 14.0, 7.0, 3.5, 1.75, 0.875, 0.4375, 0.21875, 0.109375],
  "origin": [-238375.0, 1376256.0]
});


export function initMap(geometry: string, mapConfigJSON: string) {
  //convert x/y from BNG to Spherical Mercator
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
      }),
    ],
    view: new View({
      projection: 'EPSG:27700'
    })
  });

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

  if (geometry) {
    addGeometryToMapAndCenter(geometry, 18);
  }

};

export function updateMap(geometry: string, z: number) {
  vectorSource.clear();
  if (geometry) {
    addGeometryToMapAndCenter(geometry, z);
  }
};

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

function addGeometryToMapAndCenter(geometry: string, maxZoom: number) {
  const features = new GeoJSON().readFeatures(geometry);
  if (features) {
    const mapExtent = features[0].getGeometry()!.getExtent();
    features.forEach(feature => {
      extend(mapExtent, feature.getGeometry()!.getExtent())
    });
    map.getView().fit(mapExtent, {
      padding: [20, 20, 20, 20],
      maxZoom: maxZoom
    });
    vectorSource = new VectorSource({
      features: features,
    });
    const vectorLayer = new VectorLayer({
      source: vectorSource,
      style: [iconStyle, lineStyle, polygonStyle]
    });
    map.addLayer(vectorLayer);
    vectorLayer.setVisible(true);
  }
}
