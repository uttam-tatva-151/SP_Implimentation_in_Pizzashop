$(document).ready(function () {
  let encodedEmail = new URLSearchParams(window.location.search).get(
    "forgotAction"
  );

  if (encodedEmail) {
    $("#forgotEmail").val(atob(encodedEmail));
  }
});
function togglePasswordVisibility(passwordFieldId, eyeIconId) {
  let passwordField = $("#" + passwordFieldId);
  let eyeIcon = $("#" + eyeIconId);

  if (passwordField.attr("type") === "password") {
    passwordField.attr("type", "text");
    eyeIcon.attr("src", "@Url.Content('~/images/icons/eye-slash.png')");
  } else {
    passwordField.attr("type", "password");
    eyeIcon.attr("src", "@Url.Content('~/images/icons/eye.png')");
  }
}
