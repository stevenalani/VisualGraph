window.VisualGraph = {
    
}
window.getWindowSize = function () {
    return {
        width: window.innerWidth,
        height: window.innerHeight,
    };
};
window.getSVGTransformationMatrix = function (args) {
    var ctm = $("svg#" + args.id)[0].getScreenCTM();
    return { a: ctm.a, b: ctm.b, c: ctm.c, d: ctm.d, e: ctm.e, f:ctm.f };
    
};
window.HandleMouseDown = (id) => {
    $(".vsnode").removeClass("active");
    $("#node-" + id).addClass("active");
    window.lastmouse = { };
    window.lastmouse.x = event.clientX;
    window.lastmouse.y = event.clientY;
    $("#node-" + id).on("click", function () {
        $(".vsnode").removeClass("active");
        $(this).unbind("mousemove");
    });
    $("#node-" + id).on("mousemove",function () {
        console.log("node-" + id);
        console.log(event.clientX + " " + event.clientY);
        var deltaX = window.lastmouse.x - event.clientX,
            deltaY = window.lastmouse.y - event.clientY;
        var viewbox = $(event.currentTarget).parent("svg").attr("viewBox").split(",");
        var viewBoxWidth = parseFloat(viewbox[2]);
        var viewBoxHeight = parseFloat(viewbox[3]);
        var relX = deltaX * (viewBoxWidth / window.innerWidth);
        var relY = deltaY * (viewBoxHeight / window.innerHeight);

        var position = { x: $(event.currentTarget).attr("cx"), y: $(event.currentTarget).attr("cy") }
        $(event.currentTarget).attr("cx", (parseFloat(position.x) - relX * 1.2));
        $(event.currentTarget).attr("cy", (parseFloat(position.y) - relY * 1.3));
        window.lastmouse.x = event.clientX;
        window.lastmouse.y = event.clientY;
    });
};