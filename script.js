const urlArgs = new URLSearchParams(window.location.search);
const score = urlArgs.get('score');

const first = document.getElementById("1");
const second = document.getElementById("2");
const third = document.getElementById("3");
let data;
console.log(score);

 function getJsonData() {
  const response = fetch("https://ufopeliserver.ctih.repl.co/get").then(response => response.json()).then(data => {
    first.innerHTML = data[0][0] + ": " + data[0][1];
    second.innerHTML = data[1][0] + ": " + data[1][1];
    third.innerHTML = data[2][0] + ": " + data[2][1];
  });

}

getJsonData();
const user = document.getElementById("userscore");
if(score!==null) {
  user.innerHTML = "Score: " + score;
}