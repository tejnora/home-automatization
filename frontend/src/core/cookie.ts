let resolveDomain = (): string => {
  var hostname = window.location.hostname;
  return hostname;
};

export function setCookie(name: string, value: string, domain: string) {
  var expireTime = new Date();
  expireTime.setFullYear(new Date().getFullYear() + 1);
  document.cookie =
    name +
    "=" +
    encodeURIComponent(value) +
    "; path=/; domain = " +
    domain +
    ";expires=" +
    expireTime;
}

export function getCookie(name: string) {
  name = name + "=";
  var ca = document.cookie.split(";");
  for (var i = 0; i < ca.length; i++) {
    var c = ca[i];
    while (c.charAt(0) === " ") c = c.substring(1);
    if (c.indexOf(name) === 0)
      return decodeURIComponent(c.substring(name.length, c.length));
  }
  document.cookie = ca.join(";");
}

export function usetCookie(name: string, domain: string) {
  document.cookie =
    name +
    "=; path=/; domain = " +
    domain +
    ";expires=Thu, 01 Jan 1970 00:00:00 UTC";
}

export let getLanguage = (): string => {
  let cookieLanguage = "en-us";

  if (cookieLanguage === "en-uk") {
    setLanguage("en-gb");
    return "en-gb";
  }

  return cookieLanguage;
};

export let setLanguage = (lang: string) => {
  setCookie("gc-common-lang", lang, resolveDomain());
};

export let getSessionId = () => {
  let value = getCookie("session-id");
  return value;
};

export let setSessionId = (sessingId: string) => {
  setCookie("session-id", sessingId, resolveDomain());
};

export let unsetSessionId = () => {
  usetCookie("session-id", resolveDomain());
};

export let getPernamentSessionId = () => {
  let value = getCookie("pernament-session-id");
  return value;
};

export let setPernametSessionId = (sessingId: string) => {
  setCookie("pernament-session-id", sessingId, resolveDomain());
};

export let unsetPernametSessionId = () => {
  usetCookie("pernament-session-id", resolveDomain());
};

