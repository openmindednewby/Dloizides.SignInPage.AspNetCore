// src/motion/IdleRecipe.ts
var IDLE_CAP_MS = 600;

// src/runtime/startIdle.ts
var DEFAULT_MIN_MS = 8e3;
var DEFAULT_MAX_MS = 15e3;
var DEFAULT_RUN_MS = 600;
var REDUCED_QUERY = "(prefers-reduced-motion: reduce)";
var browser = () => globalThis;
var reducedMotion = () => browser().matchMedia?.(REDUCED_QUERY).matches === true;
var tabHidden = () => browser().document?.hidden === true;
function startIdle(el, options = {}) {
  const minMs = options.minMs ?? DEFAULT_MIN_MS;
  const maxMs = Math.max(minMs, options.maxMs ?? DEFAULT_MAX_MS);
  const runMs = Math.min(options.durationMs ?? DEFAULT_RUN_MS, IDLE_CAP_MS);
  const className = options.className ?? "lk-idle";
  const random = options.random ?? Math.random;
  let timer;
  let stopped = false;
  const schedule = () => {
    timer = setTimeout(tick, minMs + random() * (maxMs - minMs));
  };
  const finish = () => {
    el.classList.remove(className);
    schedule();
  };
  function tick() {
    if (stopped) {
      return;
    }
    if (tabHidden() || reducedMotion()) {
      schedule();
      return;
    }
    el.classList.add(className);
    timer = setTimeout(finish, runMs);
  }
  if (reducedMotion()) {
    return () => void 0;
  }
  schedule();
  return () => {
    stopped = true;
    clearTimeout(timer);
    el.classList.remove(className);
  };
}

// src/runtime/mountBrand.ts
var REDUCED_QUERY2 = "(prefers-reduced-motion: reduce)";
var reducedMotion2 = () => globalThis.matchMedia?.(REDUCED_QUERY2).matches === true;
function mountBrand(host, runtime, options = {}) {
  const still = reducedMotion2();
  const mode = runtime[options.mode ?? "light" /* Light */];
  const showName = options.showName !== false && runtime.wordmark !== null;
  const wordmarkCss = showName ? runtime.wordmark?.css ?? "" : "";
  const css = still ? "" : `${wordmarkCss}${runtime.idle?.css ?? ""}`;
  const style = css !== "" ? `<style>${css}</style>` : "";
  const name = showName ? runtime.wordmark?.html ?? "" : "";
  host.innerHTML = `${style}<span class="lk-brand">${still ? mode.still : mode.intro}${name}</span>`;
  const idle = runtime.idle;
  const stopIdle = still || idle === null ? () => void 0 : startIdle(host, { durationMs: idle.durationMs, className: idle.className, ...options.idle });
  return () => {
    stopIdle();
    host.innerHTML = "";
  };
}

// src/motion/defaults.ts
var MICRO_CAP_MS = 300;

// src/motion/raster/rasterBaseCss.ts
var RASTER_PREFIX = "lkr";
var MASK = (p) => `-webkit-mask:var(--${p}-src) center/100% 100% no-repeat;mask:var(--${p}-src) center/100% 100% no-repeat`;
function layoutCss(p) {
  return `.${p}{position:relative;display:inline-block;line-height:0;vertical-align:middle}.${p}-img{display:block;max-width:100%;height:auto}.${p}-lift,.${p}-nod,.${p}-in{position:relative;display:block;transform-origin:center}.${p}-fx{position:absolute;inset:0;pointer-events:none}.${p}-glow,.${p}-haze{opacity:0;filter:blur(7px)}.${p}-tint{position:absolute;inset:0;background:var(--${p}-glow);${MASK(p)}}.${p}-sheen{overflow:hidden;${MASK(p)}}.${p}-band{position:absolute;top:0;bottom:0;left:-100%;width:0;background:linear-gradient(90deg,transparent,var(--${p}-band) 50%,transparent)}`;
}
function burstCss(p) {
  const centred = "position:absolute;left:50%;top:50%;aspect-ratio:1;border-radius:50%;opacity:0;transform:translate(-50%,-50%)";
  const ring = "radial-gradient(closest-side,transparent 52%,black 54%,black 96%,transparent 100%)";
  return `.${p}-ring{${centred};height:140%;box-sizing:border-box;border:3px solid var(--${p}-glow)}.${p}-rays{${centred};height:230%;background:repeating-conic-gradient(var(--${p}-glow) 0 3deg,transparent 3deg 22.5deg);-webkit-mask:${ring};mask:${ring}}.${p}-spark{position:absolute;inset:0;opacity:0}.${p}-spark::after{content:"";position:absolute;left:50%;top:50%;width:6px;height:6px;margin:-3px;border-radius:50%;background:var(--${p}-glow)}.${p}-shadow{position:absolute;left:8%;width:84%;top:calc(100% + 5px);height:10px;border-radius:50%;background:var(--${p}-shadow);opacity:0}.${p}-line{position:absolute;left:0;width:100%;top:calc(100% + 6px);height:3px;border-radius:1.5px;background:var(--${p}-glow);transform:scaleX(0);transform-origin:left center}`;
}
function rasterBaseCss(options = {}) {
  const p = options.prefix ?? RASTER_PREFIX;
  const vars = `.${p}{--${p}-glow:${options.glowColour ?? "currentColor"};--${p}-band:${options.bandColour ?? "white"};--${p}-shadow:${options.shadowColour ?? "currentColor"}}`;
  const paused = `.${p}-paused,.${p}-paused *,.${p}-paused *::after{animation-play-state:paused!important}`;
  const reduced = `@media (prefers-reduced-motion:reduce){.${p},.${p} *,.${p} *::after{animation:none!important}}`;
  return vars + layoutCss(p) + burstCss(p) + paused + reduced;
}

// src/runtime/startRasterMotion.ts
var REDUCED_QUERY3 = "(prefers-reduced-motion: reduce)";
var VISIBILITY = "visibilitychange";
var NOOP = { micro: () => void 0, stop: () => void 0 };
var browser2 = () => globalThis;
var restart = (host) => host.offsetWidth ?? 0;
function startRasterMotion(host, options = {}) {
  if (browser2().matchMedia?.(REDUCED_QUERY3).matches === true) {
    return NOOP;
  }
  const p = options.prefix ?? RASTER_PREFIX;
  const microClass = `${p}-play-micro`;
  const microMs = Math.min(options.microMs ?? MICRO_CAP_MS, MICRO_CAP_MS);
  const played = [options.intro !== false ? `${p}-play-intro` : "", options.idle === true ? `${p}-play-idle` : ""].filter((c) => c !== "");
  let timer;
  const sync = () => {
    if (browser2().document?.hidden === true) {
      host.classList.add(`${p}-paused`);
    } else {
      host.classList.remove(`${p}-paused`);
    }
  };
  played.forEach((c) => host.classList.add(c));
  sync();
  browser2().document?.addEventListener?.(VISIBILITY, sync);
  const micro = () => {
    clearTimeout(timer);
    host.classList.remove(microClass);
    restart(host);
    host.classList.add(microClass);
    timer = setTimeout(() => host.classList.remove(microClass), microMs);
  };
  const stop = () => {
    clearTimeout(timer);
    browser2().document?.removeEventListener?.(VISIBILITY, sync);
    [...played, microClass, `${p}-paused`].forEach((c) => host.classList.remove(c));
  };
  return { micro, stop };
}

// src/motion/raster/rasterSpec.ts
var RASTER_EASE = {
  out: "cubic-bezier(.2,.8,.2,1)",
  spring: "cubic-bezier(.34,1.4,.64,1)",
  inOut: "cubic-bezier(.65,0,.35,1)",
  linear: "linear"
};
var SPARK_COUNT = 12;
var SPARK_GROUPS = 4;
var FADE_IN = "from{opacity:0}to{opacity:1}";
var PERCENT = 100;
var n = (v) => String(Number(v.toFixed(2)));
var travel = (width) => `translateX(${n((1 + 2 * width) / width * PERCENT)}%)`;
var bandRest = (width, opacity) => `left:${n(-1.5 * width * PERCENT)}%;width:${n(width * PERCENT)}%;opacity:${n(opacity)}`;
function sweep(layer, band, ms, delayMs) {
  const frames = `from{transform:translateX(0)}to{transform:${travel(band.width)}}`;
  return { layer, frames, ms, delayMs, ease: RASTER_EASE.inOut, rest: bandRest(band.width, band.opacity) };
}
function drift(layer, band, periodMs, stops) {
  const to = travel(band.width);
  const frames = `0%{transform:translateX(0)}${stops[0]}%{transform:translateX(0);animation-timing-function:${RASTER_EASE.inOut}}${stops[1]}%{transform:${to}}100%{transform:${to}}`;
  return { layer, frames, ms: periodMs, ease: RASTER_EASE.linear, loop: true, rest: bandRest(band.width, band.opacity) };
}
function swell(layer, rest, peak, periodMs) {
  const ease = `animation-timing-function:${RASTER_EASE.inOut}`;
  return { layer, frames: `0%,100%{${rest};${ease}}50%{${peak};${ease}}`, ms: periodMs, ease: RASTER_EASE.linear, loop: true };
}
function animEndMs(anim) {
  return (anim.delayMs ?? 0) + (anim.stepMs ?? 0) * (SPARK_GROUPS - 1) + anim.ms;
}

// src/motion/raster/rasterIntros.ts
var { out, spring, inOut } = RASTER_EASE;
var CENTRED = "translate(-50%,-50%)";
var SPARK = "0%{opacity:0;transform:translate(0,0) scale(.4)}20%{opacity:1}100%{opacity:0;transform:translate(var(--dx),var(--dy)) scale(1)}";
var RASTER_INTRO_SPECS = {
  ["fade-up" /* FadeUp */]: { anims: [{ layer: "img" /* Img */, frames: "from{opacity:0;transform:translateY(14px)}to{opacity:1;transform:none}", ms: 600, ease: out }] },
  ["scale-settle" /* ScaleSettle */]: { anims: [{ layer: "img" /* Img */, frames: "from{opacity:0;transform:scale(.9)}to{opacity:1;transform:none}", ms: 700, ease: spring }] },
  ["mask-wipe" /* MaskWipe */]: { anims: [{ layer: "img" /* Img */, frames: "from{clip-path:inset(0 100% 0 0)}to{clip-path:inset(0 0 0 0)}", ms: 800, ease: inOut }] },
  ["light-sweep" /* LightSweep */]: {
    anims: [{ layer: "img" /* Img */, frames: FADE_IN, ms: 350, ease: out }, sweep("s0" /* S0 */, { width: 0.35, opacity: 0.9 }, 750, 350)]
  },
  ["focus-in" /* FocusIn */]: { anims: [{ layer: "img" /* Img */, frames: "from{opacity:0;filter:blur(10px)}to{opacity:1;filter:blur(0)}", ms: 700, ease: out }] },
  ["glow-sweep" /* GlowSweep */]: {
    anims: [
      { layer: "img" /* Img */, frames: FADE_IN, ms: 300, ease: out },
      { layer: "glow" /* Glow */, frames: "0%,100%{opacity:0}55%{opacity:.85}", ms: 800, delayMs: 300, ease: inOut },
      { layer: "in" /* In */, frames: "0%,100%{transform:none}55%{transform:scale(1.025)}", ms: 800, delayMs: 300, ease: inOut },
      sweep("s0" /* S0 */, { width: 0.55, opacity: 1 }, 700, 300)
    ]
  },
  ["punch-sweep" /* PunchSweep */]: {
    anims: [
      { layer: "in" /* In */, frames: "0%{opacity:0;transform:scale(.6)}55%{opacity:1;transform:scale(1.08)}78%{transform:scale(.98)}100%{opacity:1;transform:none}", ms: 600, ease: out },
      sweep("s0" /* S0 */, { width: 0.4, opacity: 0.95 }, 520, 600)
    ]
  },
  ["double-sweep" /* DoubleSweep */]: {
    anims: [
      { layer: "img" /* Img */, frames: FADE_IN, ms: 260, ease: out },
      sweep("s0" /* S0 */, { width: 0.13, opacity: 1 }, 330, 220),
      sweep("s1" /* S1 */, { width: 0.6, opacity: 0.95 }, 580, 560)
    ]
  },
  ["burst-settle" /* BurstSettle */]: {
    anims: [
      { layer: "in" /* In */, frames: "0%{opacity:0;transform:scale(1.25)}100%{opacity:1;transform:none}", ms: 650, ease: spring },
      { layer: "ring" /* Ring */, frames: `0%{opacity:.9;transform:${CENTRED} scale(.3)}100%{opacity:0;transform:${CENTRED} scale(2.1)}`, ms: 650, delayMs: 60, ease: out },
      { layer: "rays" /* Rays */, frames: `0%{opacity:0;transform:${CENTRED} scale(.4)}25%{opacity:.75}100%{opacity:0;transform:${CENTRED} scale(1.25)}`, ms: 600, delayMs: 40, ease: out },
      { layer: "spark" /* Spark */, frames: SPARK, ms: 720, delayMs: 120, stepMs: 40, ease: out },
      sweep("s0" /* S0 */, { width: 0.4, opacity: 0.95 }, 480, 660)
    ]
  },
  ["tilt-sweep" /* TiltSweep */]: {
    anims: [
      { layer: "in" /* In */, frames: "0%{opacity:0;transform:translateY(10px) scaleX(.2) skewY(-16deg)}45%{opacity:1}65%{transform:scaleX(1.05) skewY(2deg)}100%{opacity:1;transform:none}", ms: 620, ease: out },
      { layer: "shadow" /* Shadow */, frames: "0%{opacity:0;transform:scaleX(.3)}60%{opacity:.22;transform:scaleX(1.1)}100%{opacity:0;transform:none}", ms: 900, ease: out },
      sweep("s0" /* S0 */, { width: 0.45, opacity: 0.95 }, 520, 620)
    ]
  }
};

// src/motion/raster/rasterLoops.ts
var { out: out2 } = RASTER_EASE;
var RASTER_MICRO_SPECS = {
  ["pulse" /* Pulse */]: { anims: [{ layer: "nod" /* Nod */, frames: "0%,100%{transform:none}45%{transform:scale(1.04)}", ms: 240, ease: out2 }] },
  ["glint" /* Glint */]: { anims: [sweep("m0" /* M0 */, { width: 0.35, opacity: 0.9 }, 280, 0)] },
  ["nudge" /* Nudge */]: { anims: [{ layer: "nod" /* Nod */, frames: "0%,100%{transform:none}45%{transform:translateY(-4px)}", ms: 200, ease: out2 }] },
  ["underline-draw" /* UnderlineDraw */]: { anims: [{ layer: "line" /* Line */, frames: "from{transform:scaleX(0)}to{transform:scaleX(1)}", ms: 260, ease: out2 }] }
};
var RASTER_IDLE_SPECS = {
  ["slow-glint" /* SlowGlint */]: { periodMs: 7e3, anims: [drift("i0" /* I0 */, { width: 0.17, opacity: 0.85 }, 7e3, [0, 11])] },
  ["glow-breathe" /* GlowBreathe */]: { periodMs: 6e3, anims: [swell("haze" /* Haze */, "opacity:0", "opacity:.3", 6e3)] },
  ["sheen-drift" /* SheenDrift */]: { periodMs: 8e3, anims: [drift("i0" /* I0 */, { width: 0.8, opacity: 0.35 }, 8e3, [0, 45])] },
  ["soft-lift" /* SoftLift */]: {
    periodMs: 6500,
    anims: [swell("lift" /* Lift */, "transform:none", "transform:scale(1.018)", 6500), swell("haze" /* Haze */, "opacity:0", "opacity:.14", 6500)]
  },
  ["twin-shimmer" /* TwinShimmer */]: {
    periodMs: 8e3,
    anims: [drift("i0" /* I0 */, { width: 0.11, opacity: 0.7 }, 8e3, [0, 8]), drift("i1" /* I1 */, { width: 0.11, opacity: 0.7 }, 8e3, [4, 12])]
  }
};

// src/motion/raster/rasterPresets.ts
function animCss(a, i, s) {
  const name = `${s.p}-${s.id}-${i}`;
  const delay = a.loop ? s.idleStartMs : a.delayMs ?? 0;
  const stagger = a.stepMs ? `;animation-delay:calc(${delay}ms + var(--k) * ${a.stepMs}ms)` : "";
  const play = `.${s.p}.${s.p}-play-${s.phase} .${s.p}-${a.layer}{animation:${name} ${a.ms}ms ${a.ease} ${delay}ms ${a.loop ? "infinite" : "1"} both${stagger}}`;
  const rest = a.rest ? `.${s.p} .${s.p}-${a.layer}{${a.rest}}` : "";
  return `@keyframes ${name}{${a.frames}}${play}${rest}`;
}
function build(spec, name, phase, options) {
  const scope = { p: options.prefix ?? RASTER_PREFIX, id: `${phase}-${name}`, phase, idleStartMs: options.idleStartMs ?? 0 };
  const css = spec.anims.map((a, i) => animCss(a, i, scope)).join("");
  const durationMs = spec.periodMs ?? Math.max(...spec.anims.map(animEndMs));
  return { name, phase, durationMs, periodMs: spec.periodMs, css };
}
var pick = (table, name, phase, options) => {
  const spec = table[name];
  return spec ? build(spec, name, phase, options) : null;
};
function rasterIntro(name, options = {}) {
  return pick(RASTER_INTRO_SPECS, name, "intro" /* Intro */, options);
}
function rasterMicro(name, options = {}) {
  return pick(RASTER_MICRO_SPECS, name, "micro" /* Micro */, options);
}
function rasterIdle(name, options = {}) {
  return pick(RASTER_IDLE_SPECS, name, "idle" /* Idle */, options);
}
var unknownNames = (pairs) => pairs.flatMap(([asked, found]) => asked !== void 0 && found === null ? [asked] : []);
function rasterMotion(selection, options = {}) {
  const intro = selection.intro !== void 0 ? rasterIntro(selection.intro, options) : null;
  const micro = selection.micro !== void 0 ? rasterMicro(selection.micro, options) : null;
  const idleStartMs = options.idleStartMs ?? intro?.durationMs ?? 0;
  const idle = selection.idle !== void 0 ? rasterIdle(selection.idle, { ...options, idleStartMs }) : null;
  const unknown = unknownNames([[selection.intro, intro], [selection.micro, micro], [selection.idle, idle]]);
  const css = rasterBaseCss(options) + [intro, micro, idle].map((x) => x?.css ?? "").join("");
  return { css, introMs: intro?.durationMs ?? 0, microMs: micro?.durationMs ?? 0, periodMs: idle?.periodMs ?? 0, unknown };
}

// src/motion/raster/rasterMarkup.ts
var PERCENT2 = 100;
var HEX = 16;
var SPARK_TURN = 0.3;
var SPARK_NEAR = 0.48;
var SPARK_FAR = 0.62;
var SPARK_HEIGHT = 1.25;
var BANDS = ["s0" /* S0 */, "s1" /* S1 */, "i0" /* I0 */, "i1" /* I1 */, "m0" /* M0 */];
var attr = (v) => v.replace(/&/g, "&amp;").replace(/"/g, "&quot;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
var cssUrl = (v) => v.replace(/["'()\\\s]/g, (c) => `%${c.charCodeAt(0).toString(HEX).toUpperCase().padStart(2, "0")}`);
var n2 = (v) => String(Number(v.toFixed(1)));
function sparks(p) {
  return Array.from({ length: SPARK_COUNT }, (_, i) => {
    const a = i / SPARK_COUNT * Math.PI * 2 + SPARK_TURN;
    const r = i % 2 ? SPARK_FAR : SPARK_NEAR;
    const style = `--dx:${n2(Math.cos(a) * r * PERCENT2)}%;--dy:${n2(Math.sin(a) * SPARK_HEIGHT * PERCENT2)}%;--k:${i % SPARK_GROUPS}`;
    return `<span class="${p}-${"spark" /* Spark */}" style="${style}"></span>`;
  }).join("");
}
function rasterMarkup(src, alt, options = {}) {
  const p = options.prefix ?? RASTER_PREFIX;
  const size = options.width !== void 0 ? `;width:${options.width}` : "";
  const band = (k) => `<span class="${p}-band ${p}-${k}"></span>`;
  const glow = (k) => `<span class="${p}-fx ${p}-${k}"><span class="${p}-tint"></span></span>`;
  const img = `<span class="${p}-${"lift" /* Lift */}"><span class="${p}-${"nod" /* Nod */}"><span class="${p}-${"in" /* In */}"><img class="${p}-${"img" /* Img */}" src="${attr(src)}" alt="${attr(alt)}"></span></span></span>`;
  const burst = `<span class="${p}-fx"><span class="${p}-${"ring" /* Ring */}"></span><span class="${p}-${"rays" /* Rays */}"></span>${sparks(p)}</span>`;
  const sheen = `<span class="${p}-fx ${p}-sheen">${BANDS.map(band).join("")}</span>`;
  const host = attr(`--${p}-src:url("${cssUrl(src)}")${size}`);
  return `<span class="${p}" style="${host}">${glow("glow" /* Glow */)}${glow("haze" /* Haze */)}${burst}<span class="${p}-${"shadow" /* Shadow */}"></span>${img}${sheen}<span class="${p}-${"line" /* Line */}"></span></span>`;
}

export { mountBrand, rasterMarkup, rasterMotion, startIdle, startRasterMotion };
//# sourceMappingURL=runtime.js.map
//# sourceMappingURL=runtime.js.map