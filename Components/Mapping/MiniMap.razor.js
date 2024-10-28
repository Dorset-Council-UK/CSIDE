const bngTilegrid = new ol.tilegrid.TileGrid({
  "resolutions": [896.0, 448.0, 224.0, 112.0, 56.0, 28.0, 14.0, 7.0, 3.5, 1.75, 0.875, 0.4375, 0.21875, 0.109375],
  "origin": [-238375.0, 1376256.0]
});

const roadBasemap = new ol.source.XYZ(
  {
    url: 'https://api.os.uk/maps/raster/v1/zxy/Road_27700/{z}/{x}/{y}.png?key=J3H6E7O9J3cZuUvkjdOASdbGDAmQxjZJ',
    attributions: `© Crown copyright and database rights ${new Date().getFullYear()} <a href="https://os.uk" target="_blank">OS</a> AC0000830671.`,
    attributionsCollapsible: false,
    tileGrid: bngTilegrid,
    projection: 'EPSG:27700'
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

let vectorSource;

export function initMap(mapId, x, y, z, geometry) {
  //convert x/y from BNG to Spherical Mercator
  proj4.defs(
    'EPSG:27700',
    '+proj=tmerc +lat_0=49 +lon_0=-2 +k=0.9996012717 ' +
    '+x_0=400000 +y_0=-100000 +ellps=airy ' +
    '+towgs84=446.448,-125.157,542.06,0.15,0.247,0.842,-20.489 ' +
    '+units=m +no_defs',
  );
  ol.proj.proj4.register(proj4);
  const map = new ol.Map({
    target: mapId,
    layers: [
      new ol.layer.Tile({
        source: roadBasemap
      }),
    ],
    view: new ol.View({
      center: [x,y],
      zoom: z,
      projection: 'EPSG:27700'
    })
  });

  if (geometry) {
    const features = new ol.format.GeoJSON().readFeatures(geometry);
    const mapExtent = features[0].getGeometry().getExtent();
    features.forEach(feature => {
      ol.extent.extend(mapExtent, feature.getGeometry().getExtent())
    });
    map.getView().fit(mapExtent, {
      padding: [20, 20, 20, 20],
      maxZoom: z
    });
    vectorSource = new ol.source.Vector({
      features: features,
    });
    const vectorLayer = new ol.layer.Vector({
      source: vectorSource,
      style: iconStyle
    });
    map.addLayer(vectorLayer);
    vectorLayer.setVisible(true);

  }

};
