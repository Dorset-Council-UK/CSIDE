import { Map, View } from "ol";
import EditBar from "ol-ext/control/EditBar";
import Feature from "ol/Feature";
import { extend } from "ol/extent";
import { Control } from 'ol/control';
import { GeoJSON } from "ol/format";
import { GeometryCollection, LineString, MultiLineString, MultiPoint, Point, SimpleGeometry } from "ol/geom";
import { Draw, Select, Snap } from "ol/interaction";
import { Tile as TileLayer, Vector as VectorLayer } from "ol/layer";
import { register } from "ol/proj/proj4";
import { TileWMS } from "ol/source";
import VectorSource from "ol/source/Vector";
import XYZ from "ol/source/XYZ";
import { Circle, Fill, Icon, Stroke, Style } from "ol/style";
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
    color: '#ffff00',
    width: 4
  })
});
const polySelectionStyle = new Style({
  stroke: new Stroke({
    color: '#ff0000',
    width: 4
  }),
  fill: new Fill({
    color: 'rgba(0,0,0,0.1)'
  })
});
const polygonDrawStyle = new Style({
  stroke: new Stroke({
    color: '#ffff00',
    width: 4
  }),
  fill: new Fill({
    color: 'rgba(0,0,0,0.1)'
  })
});
const vertexStyle = new Style({
  image: new Circle({
    radius: 5,
    fill: new Fill({
      color: "red",
    }),
  }),
  geometry: (feature) => {
    const geometry = feature.getGeometry();
    let coordinates: number[][] = [];
    const geomType = geometry?.getType();
    if (geomType === 'MultiLineString') {
      ((geometry as MultiLineString).getCoordinates() as number[][][]).forEach(coord => {
        coordinates = coordinates.concat(coord);
      });
    } else if (geomType === 'GeometryCollection') {
      (geometry as GeometryCollection).getGeometries().forEach(geom => {
        coordinates = coordinates.concat((geom as LineString).getCoordinates());
      });
    } else {
      coordinates = (geometry as SimpleGeometry)?.getCoordinates()!;
    }
    return new MultiPoint(coordinates);
  }
});
const selectedStyle = [new Style({
  stroke: new Stroke({
    color: '#000000',
    width: 6
  })
}), new Style({
  stroke: new Stroke({
    color: '#ffff00',
    width: 4
  })
}),
  vertexStyle
];

let map: Map;
let editSource: VectorSource;
const bngTilegrid: TileGrid = new TileGrid({
  "resolutions": [896.0, 448.0, 224.0, 112.0, 56.0, 28.0, 14.0, 7.0, 3.5, 1.75, 0.875, 0.4375, 0.21875, 0.109375],
  "origin": [-238375.0, 1376256.0]
});
let originalGeometry: string;


export function initMap(geomType: string, geometry: string, mapConfigJSON: string, component: any) {

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
  originalGeometry = geometry;
  initGeometry(geometry);

  initEditing(geomType, component);
}

function initEditing(geomType: string, component: any) {
  
  switch (geomType) {
    case "Point":
      //create point editing tools
      const vectorLayer = new VectorLayer({
        source: editSource,
        style: iconStyle
      });
      map.addLayer(vectorLayer);
      const drawInteraction = new Draw({
        source: editSource,
        type: 'Point',
      });
      map.addInteraction(drawInteraction);
      drawInteraction.on('drawend', (e) => {
        editSource.clear();
        //convert to GeoJSON
        const geoJson = new GeoJSON().writeFeatures([e.feature]);
        component.invokeMethodAsync('OnDrawEnd', geoJson);
      })
      map.getViewport().style.cursor = 'crosshair';

      break;

    case "MultiPoint":
      //create point editing tools
      break;

    case "Line":
    case "Line+RoutePicker":
      setupLineEditing(geomType, component);
      break;

    case "Polygon":
      setupPolygonEditing(component);
      break;

    case "PolygonSelector":
      //create polygon editing tools
      const dmmoLayer = new VectorLayer({
        source: editSource,
        style: lineStyle
      });
      map.addLayer(dmmoLayer);
      let selectorSource = new VectorSource();
      const polyVectorLayer = new VectorLayer({
        source: selectorSource,
        style: polySelectionStyle
      });
      map.addLayer(polyVectorLayer);
      const polySelectionInteraction = new Draw({
        source: selectorSource,
        type: 'Polygon',
      });
      map.addInteraction(polySelectionInteraction);
      polySelectionInteraction.on('drawend', (e) => {
        selectorSource.clear();
        //convert to GeoJSON
        const geoJson = new GeoJSON().writeFeature(e.feature);
        component.invokeMethodAsync('OnDrawEnd', geoJson);
      })
      map.getViewport().style.cursor = 'crosshair';

      break;
  }
}

function initGeometry(geometry:string) {
  editSource = new VectorSource();
  if (geometry) {
    loadFeaturesFromGeoJSON(geometry);
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

// Helper Functions

function loadFeaturesFromGeoJSON(geometry: string) {
  const features = new GeoJSON().readFeatures(geometry);
  if (features && features.length > 0) {
    const mapExtent = features[0].getGeometry()!.getExtent();
    features.forEach(feature => {
      extend(mapExtent, feature.getGeometry()!.getExtent())
    });
    map.getView().fit(mapExtent, {
      padding: [20, 20, 20, 20],
      maxZoom: 16
    });

    addFeaturesToSource(features);
  }
}

function addFeaturesToSource(features: Feature[]) {
  features.forEach(feature => {
    const geom = feature.getGeometry();
    if (geom) {
      if (geom.getType() === 'MultiLineString') {
        const lineStrings = (geom as MultiLineString).getLineStrings();
        lineStrings.forEach(lineString => {
          const lineFeature = new Feature({ geometry: lineString });
          editSource.addFeature(lineFeature);
        });
      } else {
        editSource.addFeature(feature);
      }
    }
  });
}

function serializeFeaturesAsGeoJSON(features: Feature[]): string {
  return new GeoJSON().writeFeatures(features);
}

function createDrawEndHandler(component: any, includeExistingFeatures: boolean = true) {
  return (e: any) => {
    const features = includeExistingFeatures 
      ? [...editSource.getFeatures(), e.feature] 
      : editSource.getFeatures();
    const geoJson = serializeFeaturesAsGeoJSON(features);
    component.invokeMethodAsync('OnDrawEnd', geoJson);
  };
}

function createModifyEndHandler(component: any) {
  return (e: any) => {
    const geoJson = serializeFeaturesAsGeoJSON(editSource.getFeatures());
    component.invokeMethodAsync('OnDrawEnd', geoJson);
  };
}

function createDeleteEndHandler(component: any) {
  return (e: any) => {
    editSource.removeFeatures(e.features);
    const geoJson = serializeFeaturesAsGeoJSON(editSource.getFeatures());
    component.invokeMethodAsync('OnDrawEnd', geoJson);
  };
}

function setupPointerCursor(layer: VectorLayer<any>) {
  map.on('pointermove', (e) => {
    const pixel = map.getEventPixel(e.originalEvent);
    const hit = map.hasFeatureAtPixel(pixel, { 
      layerFilter: (lyr) => lyr === layer 
    });
    map.getViewport().style.cursor = hit ? 'pointer' : '';
  });
}

function createEditBarConfig(geomType: 'line' | 'polygon', select: Select, pickRoute?: PickRoute): any {
  const baseConfig: any = {
    interactions: {
      Select: select,
      DrawLine: false,
      DrawPolygon: false,
      DrawPoint: false,
      DrawRegular: false,
      DrawHole: false,
      Info: false,
      Offset: false,
      Split: false,
    },
    source: editSource
  };

  if (geomType === 'line') {
    baseConfig.interactions.DrawLine = 'Draw line';
    baseConfig.interactions.DrawPoint = pickRoute || false;
  } else if (geomType === 'polygon') {
    baseConfig.interactions.DrawPolygon = 'Draw polygon';
  }

  return baseConfig;
}

function setInitialEditInteraction(edit: EditBar, drawInteractionName: string) {
  const features = editSource.getFeatures();
  if (features && features.length > 0) {
    edit.getInteraction('Select').setActive(true);
  } else {
    edit.getInteraction(drawInteractionName).setActive(true);
  }
}

function setupLineEditing(geomType: string, component: any) {
  const lineVectorLayer = new VectorLayer({
    source: editSource,
    style: lineStyle
  });
  map.addLayer(lineVectorLayer);

  const select = new Select({ style: selectedStyle });
  const pickRoute = geomType === 'Line+RoutePicker' ? new PickRoute() : undefined;
  
  if (pickRoute) {
    pickRoute.on('drawend', async (e) => {
      const selectedRoute: string = await component.invokeMethodAsync('GetRoute', (e.feature.getGeometry() as Point).getCoordinates());
      if (selectedRoute != null) {
        editSource.addFeatures(new GeoJSON().readFeatures(selectedRoute));
      }
      const geoJson = serializeFeaturesAsGeoJSON(editSource.getFeatures());
      component.invokeMethodAsync('OnDrawEnd', geoJson);
    });
  }

  const revert = new RevertControl({}, component);
  const edit = new EditBar(createEditBarConfig('line', select, pickRoute));
  edit.addControl(revert);
  map.addControl(edit);

  setInitialEditInteraction(edit, 'DrawLine');

  const sourceSnap = new Snap({ source: editSource });
  const snappableRoutes = new VectorSource();
  const routeSnap = new Snap({ source: snappableRoutes });
  map.addInteraction(sourceSnap);
  map.addInteraction(routeSnap);

  edit.getInteraction('ModifySelect').on('modifyend' as any, createModifyEndHandler(component));
  edit.getInteraction('DrawLine').on('drawend' as any, createDrawEndHandler(component));
  edit.getInteraction('Delete').on('deleteend' as any, createDeleteEndHandler(component));

  map.on('moveend', async (e) => {
    const view = map.getView();
    if (view.getZoom()! < 15) { return; }
    const extent = view.calculateExtent(map.getSize());
    const routesGeojson = await component.invokeMethodAsync('GetSnappables', extent);
    snappableRoutes.clear();
    snappableRoutes.addFeatures(new GeoJSON().readFeatures(routesGeojson));
  });

  setupPointerCursor(lineVectorLayer);
}

function setupPolygonEditing(component: any) {
  const polygonVectorLayer = new VectorLayer({
    source: editSource,
    style: polygonDrawStyle
  });
  map.addLayer(polygonVectorLayer);

  const select = new Select({ style: selectedStyle });
  const revert = new RevertControl({}, component);
  const edit = new EditBar(createEditBarConfig('polygon', select));
  edit.addControl(revert);
  map.addControl(edit);

  setInitialEditInteraction(edit, 'DrawPolygon');

  edit.getInteraction('ModifySelect').on('modifyend' as any, createModifyEndHandler(component));
  edit.getInteraction('DrawPolygon').on('drawend' as any, createDrawEndHandler(component));

  setupPointerCursor(polygonVectorLayer);
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

class RevertControl extends Control {
  /**
   * @param {Object} [opt_options] Control options.
   */
  constructor(opt_options: any, component:any) {
    const options = opt_options || {};

    const button = document.createElement('button');
    button.type = 'button';
    button.innerHTML = '<span class="bi bi-arrow-counterclockwise"></span>';

    const element = document.createElement('div');
    element.className = 'revert-button ol-control';
    element.appendChild(button);

    super({
      element: element,
      target: options.target,
    });

    button.addEventListener('click', this.revertEdits.bind(this, component), false);
  }

  revertEdits(component: any) {
    editSource.clear();
    loadFeaturesFromGeoJSON(originalGeometry);
    editSource.changed();
    const geoJson = serializeFeaturesAsGeoJSON(editSource.getFeatures());
    component.invokeMethodAsync('OnDrawEnd', geoJson);
  }
}

export function clearDrawnGeometries() {
  editSource.clear();
};
