const bngTilegrid = new ol.tilegrid.TileGrid({
  "resolutions": [896.0, 448.0, 224.0, 112.0, 56.0, 28.0, 14.0, 7.0, 3.5, 1.75, 0.875, 0.4375, 0.21875, 0.109375],
  "origin": [-238375.0, 1376256.0]
});

const computedStyles = window.getComputedStyle(document.body);
const themeColor = computedStyles.getPropertyValue('--primary-color'); //get the primary color from the CSS variables
const iconStyle = new ol.style.Style({
  image: new ol.style.Icon({
    anchor: [0.5, 32], //50% from left, 32px (height of icon) up. Pins the tail of the icon to the right location, rather than the center
    anchorXUnits: "fraction",
    anchorYUnits: "pixels",
    crossOrigin: 'anonymous',
    color: themeColor,
    src: `/img/map-pin.svg`, //TODO this won't work in contexts where the app isn't running on root
  }),
});
const lineStyle = new ol.style.Style({
  stroke: new ol.style.Stroke({
    color: themeColor,
    width: 3
  })
});
let map;
let editSource;
export function initMap(geomType, mapConfigJSON, component) {

  setProj4Defs();

  const mapConfig = JSON.parse(mapConfigJSON);

  const basemap = new ol.source.XYZ(
    {
      url: `https://api.os.uk/maps/raster/v1/zxy/${mapConfig.basemap}_27700/{z}/{x}/{y}.png?key=${mapConfig.osMapsAPIKey}`,
      attributions: `© Crown copyright and database rights ${new Date().getFullYear()} <a href="https://os.uk" target="_blank">OS</a> ${mapConfig.osLicenceNumber}.`,
      attributionsCollapsible: false,
      tileGrid: bngTilegrid,
      projection: 'EPSG:27700'
    });

  map = new ol.Map({
    target: mapConfig.mapId,
    layers: [
      new ol.layer.Tile({
        source: basemap
      }),
    ],
    view: new ol.View({
      projection: 'EPSG:27700'
    })
  });
  
  map.getView().fit(mapConfig.bounds, { size: map.getSize() });

  if (mapConfig.overlays) {
    mapConfig.overlays.forEach(overlay => {
      const overlaySource = new ol.source.TileWMS({
        url: overlay.mapServiceBaseURL,
        params: { 'LAYERS': overlay.layerName },
      });
      const layer = new ol.layer.Tile({ source: overlaySource });
      map.addLayer(layer);
    });
  }

  editSource = new ol.source.Vector();
  switch (geomType) {
    case "Point":
      //create point editing tools
      const vectorLayer = new ol.layer.Vector({
        source: editSource,
        style: iconStyle
      });
      map.addLayer(vectorLayer);
      const drawInteraction = new ol.interaction.Draw({
        source: editSource,
        type: 'Point',
      });
      map.addInteraction(drawInteraction);
      drawInteraction.on('drawend', (e) => {
        editSource.clear();
        //convert to GeoJSON
        const geoJson = new ol.format.GeoJSON().writeFeatures([e.feature]);
        component.invokeMethodAsync('OnDrawEnd', geoJson);
        /*editSource.addFeature(e.feature);*/
      })
      break;
    case "MultiPoint":
      //create point editing tools
      break;
    case "Line":
      //create line editing tools
      break;
    case "Polygon":
      //create polygon editing tools
      break;

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
  ol.proj.proj4.register(proj4);
}
export function clearDrawnGeometries() {
  editSource.clear();
};
