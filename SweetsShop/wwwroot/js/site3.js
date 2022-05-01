
var sumValue = document.getElementById("sum");
function plusCount(count, button2, idButton1, cost) {
    var sum = parseInt(getSavedValue("sum"));
    sum += cost;
    localStorage.setItem("sum", sum);
    var kol = document.getElementById(String(count));
    var button1 = document.getElementById(String(idButton1));
    if (parseInt(kol.value) >= 1) {
        button2.type = "button";
        button1.type = "button";
    }
    if (parseInt(kol.value) == 0) {
        button1.type = "submit";
        button2.type = "submit";
    }
    if (parseInt(kol.value) < 99) kol.value = String(parseInt(getSavedValue(count)) + 1);
    sumValue.value = localStorage.getItem("sum") + " rub";
}
function removeItem(count, cost) {
    var sum = parseInt(getSavedValue("sum"));
    var kol = document.getElementById(String(count));
    sum -= cost * kol.value;
    localStorage.setItem("sum", sum);
    sumValue.value = localStorage.getItem("sum") + " rub";
    localStorage.removeItem(count);
}
function minusCount(count, button1, idButton2, cost) {
    var sum = parseInt(getSavedValue("sum"));
    var kol = document.getElementById(String(count));
    var button2 = document.getElementById(String(idButton2));
    if (sum > 0 && parseInt(kol.value) != 0) {
        sum -= cost;
        localStorage.setItem("sum", sum);
    }
    if (parseInt(kol.value) == 1) {
        button1.type = "submit";
        button2.type = "submit";
    }
    if (parseInt(kol.value) == 0 || parseInt(kol.value) > 1) {
        button1.type = "button";
        button2.type = "button";
    }
    if (parseInt(kol.value) > 0) kol.value = String(parseInt(getSavedValue(count)) - 1);
    sumValue.value = localStorage.getItem("sum") + " rub";
}
function saveValue(name) {
    var e = document.getElementById(String(name));
    var id = e.id;
    var val = e.value;
    localStorage.setItem(id, val);
}
function getSavedValue(v) {
    if (localStorage.getItem(v) === null) {
        return "0";
    }
    return localStorage.getItem(v);
}

