function UnityProgress (dom) {
  this.progress = 0.0;
  this.message = "";
  this.dom = dom;
  var parent = dom.parentNode;

  createjs.CSSPlugin.install(createjs.Tween);
  createjs.Ticker.setFPS(60);
  
  this.SetProgress = function (progress) { 
    if (this.progress < progress)
      this.progress = progress; 
    if (progress == 1) {
      this.SetMessage("Loading...");
      document.getElementById("spinner").style.display = "inherit";
      document.getElementById("bgBar").style.display = "none";
      document.getElementById("progressBar").style.display = "none";
    } 
    this.Update();
  }
  this.SetMessage = function (message) { 
    this.message = message; 
    this.Update();
  }
  this.Clear = function() {
    document.getElementById("loadingBox").style.display = "none";
    document.getElementById("tips-slideshow").style.display = "none";
  }
  this.Update = function() {
    var length = 200 * Math.min(this.progress, 1);
    bar = document.getElementById("progressBar");

    createjs.Tween.removeTweens(bar);
    createjs.Tween.get(bar).to({width: length}, 500, createjs.Ease.sineOut);
    
    // bar.style.width = length + "px";
    function replacer(match, p1, offset, string) {
      return p1;
    }
    document.getElementById("loadingInfo").innerHTML = this.message.replace(/\d+/g, replacer);
  }
  this.Update ();
}