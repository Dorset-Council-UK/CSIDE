export function initGallery() {
  document.querySelectorAll('#media-gallery .blueimp-img').forEach(img => {
    img.addEventListener('click', (event) => {
      var target = event.target;
      var link = target.src ? target.parentNode : target
      var options = { index: link, event: event }
      var links = document.querySelectorAll('#media-gallery .blueimp-img');
      blueimp.Gallery(links, options)
    });
  });
}