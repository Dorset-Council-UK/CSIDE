let watchID: number;
let bestAccuracy: number | undefined;
let bestLat: number | undefined;
let bestLon: number | undefined;
let locationTimeout: number;
export function getLocation(component: any) {
  const options = {
    enableHighAccuracy: true,
    timeout: 60000
  };
  watchID = navigator.geolocation.watchPosition((e) => geolocationSuccess(e,component), (e) => geolocationFailure(e,component), options);
  locationTimeout = window.setTimeout(function () {
    navigator.geolocation.clearWatch(watchID);
    console.info('After 30 seconds the best accuracy we could achieve was ' + Math.round((bestAccuracy ? bestAccuracy : 0) * 100) / 100 + 'm');
    if (bestAccuracy && bestAccuracy <= 150) {
      //good enough?
      const returnValue = [bestLat ? bestLat : 0, bestLon ? bestLon : 0, bestAccuracy ? bestAccuracy : 0];
      component.invokeMethodAsync('OnLocationSuccess', returnValue);
    } else {
      //not good enough
      const returnValue = [null, null, bestAccuracy ? bestAccuracy : 0]
      component.invokeMethodAsync('OnLocationFailure', returnValue);
    }
  }, 30000)
};
function geolocationSuccess(pos: GeolocationPosition, component: any) {
  if (bestAccuracy === undefined) {
    //first location
    console.info("First location found. Accuracy is " + Math.round(pos.coords.accuracy * 100) / 100 + 'm');

    bestLat = pos.coords.latitude;
    bestLon = pos.coords.longitude;
    bestAccuracy = Math.round(pos.coords.accuracy * 100) / 100;
  } else {
    //see if this new location beats the previous one
    if (pos.coords.accuracy < bestAccuracy) {
      console.info("More accurate position found. Accuracy was " + Math.round(bestAccuracy * 100) / 100 + "m, now " + Math.round(pos.coords.accuracy * 100) / 100 + 'm');
      bestLat = pos.coords.latitude;
      bestLon = pos.coords.longitude;
      bestAccuracy = Math.round(pos.coords.accuracy * 100) / 100;
    }
  }
  //update live accuracy
  component.invokeMethodAsync('OnLocationUpdate',  bestAccuracy);
  //check accuracy. If good enough, cancel the timeout and return the coords
  if (bestAccuracy < 2) {
    console.info('Accuracy less than 2 metres, this is good enough so we\'ll take that result');
    navigator.geolocation.clearWatch(watchID);
    window.clearTimeout(locationTimeout);
    const returnValue = [bestLat ? bestLat : 0, bestLon ? bestLon : 0, bestAccuracy ? bestAccuracy : 0];
    component.invokeMethodAsync('OnLocationSuccess', returnValue);
  }

}

function geolocationFailure(err: GeolocationPositionError, component: any) {
  console.log(err);
  component.invokeMethodAsync('OnLocationFailure', null);
}

export function resetGeolocation () {
  bestLat = undefined;
  bestLon = undefined;
  bestAccuracy = undefined;
}