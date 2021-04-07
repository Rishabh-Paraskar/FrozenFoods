var x = 0;
function Quantity() {
    x++;
    if (x < 0) {
        x = 0;
    }
    document.getElementById('output').innerHTML = x;
};

function QuantityMinus() {
    x--;
    if (x < 0) {
        document.getElementById('output').innerHTML = 0;
    } else {
        document.getElementById('output').innerHTML = x;
    }
};