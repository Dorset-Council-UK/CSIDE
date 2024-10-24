let roadBasemap = new ol.source.XYZ(
  {
    url: 'https://api.os.uk/maps/raster/v1/zxy/Road_3857/{z}/{x}/{y}.png?key=J3H6E7O9J3cZuUvkjdOASdbGDAmQxjZJ',
    attributions: `© Crown copyright and database rights ${new Date().getFullYear()} <a href="https://os.uk" target="_blank">OS</a> AC0000830671.`,
    attributionsCollapsible: false
  });

let vectorSource;

export function initMap(mapId, x, y, z) {
  //convert x/y from BNG to Spherical Mercator
  proj4.defs(
    'EPSG:27700',
    '+proj=tmerc +lat_0=49 +lon_0=-2 +k=0.9996012717 ' +
    '+x_0=400000 +y_0=-100000 +ellps=airy ' +
    '+towgs84=446.448,-125.157,542.06,0.15,0.247,0.842,-20.489 ' +
    '+units=m +no_defs',
  );
  ol.proj.proj4.register(proj4);
  const coords = ol.proj.transform([x,y],'EPSG:27700','EPSG:3857')
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


};
