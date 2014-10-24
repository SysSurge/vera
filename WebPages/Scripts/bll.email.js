/*! Vera VAF 1.0 license: https://github.com/sparkledb/vera */

/* Returns true if valid e-mail */
function IsEmail(email) {
    var regexUnicode = /^\S+@\S+\.\S+$/;
    return regexUnicode.test(email);
}
