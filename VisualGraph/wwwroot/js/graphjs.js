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
$(document).ready(() => {
    $(document).on("contextmenu", (e) => { e.preventDefault(); return false });
});
