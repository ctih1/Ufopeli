const urlArgs = new URLSearchParams(window.location.search);
const score = urlArgs.get('score');

const first = document.getElementById("1");
const second = document.getElementById("2");
const third = document.getElementById("3");
let data;
console.log(score);

function convert(obj) {
  return Object.keys(obj).map(key => ({
      name: key,
      value: obj[key],
  }));
}
 function getJsonData() {
  const response = fetch("https://ufopeliserver.ctih.repl.co/get").then(response => response.json()).then(data => {
    this.data = convert(data);
    first.innerHTML = this.data[0].name + ": " +  this.data[0].value;
    second.innerHTML = this.data[1].name + ": " +  this.data[1].value;
    third.innerHTML = this.data[2].name + ": " +  this.data[2].value;
  });

}

getJsonData();
const user = document.getElementById("userscore");
if(score!==null) {
  user.innerHTML = "Score: " + score;
}