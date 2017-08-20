
window.onload = function () {
    document.getElementById('menu').style.display = 'block';
    var boxHeight = document.getElementsByClassName('box')[0].clientHeight;
    var screenHeight = screen.height;
    //if (boxHeight < screenHeight) {
    //    document.getElementsByClassName('box')[0].style.height = screenHeight.toString() + 'px';
    //}
    var slideout = new Slideout({
        'panel': document.getElementById('panel'),
        'menu': document.getElementById('menu'),
        'side': 'right'
    });

    document.querySelector('.js-slideout-toggle').addEventListener('click', function () {
        slideout.toggle();
    });

    document.querySelector('.menu').addEventListener('click', function (eve) {
        if (eve.target.nodeName === 'A') { slideout.close(); }
    });

    var intTotalShortListed = document.getElementById('hdnTotalSelectedNames') != null ? document.getElementById('hdnTotalSelectedNames').value : 0;
    if (intTotalShortListed != undefined && intTotalShortListed > 0) {
        document.getElementById('shortListedContainer').style.display = 'block';
        document.getElementById('spnTotal').innerHTML = intTotalShortListed;
        setTimeout(function () {
            document.getElementsByClassName('shortlisted')[0].style.display = 'none';
        }, 2000);
    } else if (document.getElementById('shortListedContainer') != null) {
        document.getElementById('shortListedContainer').style.display = 'none';
    }
};