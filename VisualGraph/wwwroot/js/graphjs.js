window.VSSVG = null;
window.getWindowSize = function () {
    return {
        width: window.innerWidth,
        height: window.innerHeight,
    };
};
window.getSVGTransformationMatrix = function (args) {
    var element = $("svg#" + args.id)[0];
    var ctm = element.getScreenCTM();
    return {
        name: args.id,
        a: ctm.a,
        b: ctm.b,
        c: ctm.c,
        d: ctm.d,
        e: ctm.e,
        f: ctm.f,
        width: element.clientWidth,
        height: element.clientHeight,
        parentheight: (element.parentElement.clientHeight - element.getBoundingClientRect().y),
        parentwidth: element.parentElement.clientWidth
    };
    
};
window.getAllSVGTransformationMatrices = function () {
    let cout = [];
    $(".vssvg").each(function (index, element) {
        var ctm = element.getScreenCTM();
        let obj = {
            name: element.id,
            a: ctm.a,
            b: ctm.b,
            c: ctm.c,
            d: ctm.d,
            e: ctm.e,
            f: ctm.f,
            width: element.innerWidth,
            height: element.innerWidth,
            parentheight: element.parentElement.innerHeight,
            parentwidth: element.parentElement.innerWidth
        };
        cout.push(obj);
    });
    return cout;
};
window.InitPanZoom = function (dotnetref, svgid) {
    if (this.document.getElementById(svgid) == null) return;
    if (window.VSSVG != null)
        window.VSSVG.destroy();

    window.VSSVG = svgPanZoom(`#${svgid}`, {
        minZoom: 0.2,
        maxZoom: 10
    });
    
    window.VSSVG.setOnPan(function () { dotnetref.invokeMethodAsync('UpdateDisplay', window.GetPanZoomValues(svgid)); });
};
window.UpdateBBox = function () {
    if (window.VSSVG == null) return;
    window.VSSVG.updateBBox();
};
window.Fit = function () {
    if (window.VSSVG == null) return; 
    window.VSSVG.updateBBox();
    window.VSSVG.fit();
}
window.Resize = function () {
    if (window.VSSVG == null) return;
    window.VSSVG.resize();
}
window.Center = function () {
    if (window.VSSVG == null) return; 
    window.VSSVG.center();
}
window.DestroyPanZoom = function () {
    if (window.VSSVG == null) return; 
    window.VSSVG.destroy();
}
window.DisablePan = function () {
    if (window.VSSVG == null) return; 
    window.VSSVG.disablePan();
}
window.EnablePan = function () {
    if (window.VSSVG == null) return; 
    window.VSSVG.enablePan();
}
window.GetSvgContainerSizes = function (svgid) {
    if (this.document.getElementById(svgid) == null) return;
    var svgelement = jQuery(`#${svgid}`);
    var offsetLeft = svgelement.offset().left;
    var offsetTop = svgelement.offset().top;
    var conteinerWidth = window.innerWidth - offsetLeft;
    var conteinerHeight = window.innerHeight - offsetTop;
    var obj = {
        ParentWidth: conteinerWidth,
        ParentHeight: conteinerHeight,
        OffsetLeft: offsetLeft,
        OffsetTop: offsetTop,
    }
    return obj;
}
window.GetPanZoomValues = function (svgid) {
    if (window.VSSVG == null || this.document.getElementById(svgid) == null) return;
    var svgelement = jQuery(`#${svgid}`);
    var offsetLeft = svgelement.offset().left;
    var offsetTop = svgelement.offset().top;
    var pan = window.VSSVG.getPan();
    var zoom = window.VSSVG.getSizes().realZoom;
    var panZoomHeight = VSSVG.getSizes().height;
    var viewboxHeight = VSSVG.getSizes().viewBox.height;
    var panZoomWidth = VSSVG.getSizes().width;
    var viewboxWidth = VSSVG.getSizes().viewBox.width;
    var conteinerWidth = window.innerWidth - offsetLeft;
    var conteinerHeight = window.innerHeight - offsetTop;
    var obj = {
        PanX: pan.x,
        PanY: pan.y,
        Zoom: zoom,
        PanZoomHeight: panZoomHeight,
        PanZoomWidth: panZoomWidth,
        ViewBoxHeight: viewboxHeight,
        ViewBoxWidth: viewboxWidth,
        centerX: panZoomWidth / 2 - pan.x,
        centerY: panZoomHeight / 2 - pan.y,
        OffsetLeft: offsetLeft,
        OffsetTop: offsetTop,
        ParentWidth: conteinerWidth,
        ParentHeight: conteinerHeight,
    }
    return obj;
}
window.GetTranslatedMousePos = function (args) {
    var x = args.x;
    var y = args.y;
    var svgDropPoint = document.getElementById(args.id).createSVGPoint();

    svgDropPoint.x = x;
    svgDropPoint.y = y;
    svgDropPoint = svgDropPoint.matrixTransform(jQuery(".svg-pan-zoom_viewport")[0].getCTM().inverse());
    return {
        X: svgDropPoint.x,
        Y: svgDropPoint.y,
    };
}
$(document).ready(() => {
   $(document).on("contextmenu", (e) => { e.preventDefault(); return false; });
});
