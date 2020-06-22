var urls = [];
var currentIndex = 0;


function setHeight(id) {
    var articles = getFrameDoc(id).getElementsByClassName('article');
    var article = articles[articles.length - 1];
    var heightStr = article.style.height;
    heightStr = heightStr.substr(0, heightStr.length - 2);
    var top = (+heightStr + 30) * articles.length + 100;
    document.getElementById(id).height = top + 'px';

}

function initFrame(id) {
    
    setHeight(id);
    // setTimeout(function () {
    // var frame=getFrameDoc(id);
    // var bodyWidth=document.body.offsetWidth;
    // var frameWidthStr=frame.getElementsByClassName('article')[0].style.width;
    // frameWidthStr = +frameWidthStr.substr(0, frameWidthStr.length - 2)+16;
    // //frame.body.style.margin = 0;
    // frame.body.style.zoom =(bodyWidth/+frameWidthStr)*100+ "%" ;
    // },20);
}

function getFrameDoc(id) {
    var frame = document.getElementById(id);
    var innerDoc = frame.contentWindow || frame.contentDocument;
    return innerDoc.document;

}

function moveBy(i) {
    if (!(i > 0) && !(i < 0) && i !== 0)
        return;
    currentIndex += i;
    if (currentIndex < 0)
        currentIndex = 0;
    if (currentIndex >= urls.length)
        currentIndex = urls.length - 1;
    var url = urls[currentIndex];
    // var current=window.location.href;
    // var curIndex=current.indexOf("Mail");
    // if (curIndex>=0)
    // {
    // var startCurrent=current.substr(0,curIndex);
    // url = startCurrent+url.substr(url.indexOf("Mail"));
    // }
    try {

        document.getElementById('frames').scroll(0, 0);
    }
    catch (e) {

    }
    document.getElementById('url').innerText = url.substr(url.lastIndexOf("\\") + 1);
    document.getElementById('text').value = currentIndex;
    document.getElementById('frame').src = url;
}

function init() {
    var input = document.getElementById("text");
    input.addEventListener("keyup", function (event) {
        if (event.keyCode === 13) {
            event.preventDefault();
            moveBy(+(document.getElementById("text").value) - currentIndex);
        }
    });
    moveBy(0);
}
