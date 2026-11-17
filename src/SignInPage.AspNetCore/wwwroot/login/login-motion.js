import { rasterMarkup, rasterMotion, startRasterMotion } from './vendor/logo-kit-runtime.js';
import { wireLoginSubmit } from './login-submit.js';

const REDUCED_QUERY = '(prefers-reduced-motion: reduce)';

function cssToken(doc, win, name) {
  return win.getComputedStyle(doc.documentElement).getPropertyValue(name).trim();
}

export function mountLoginLogo(doc, win) {
  const slot = doc.getElementById('login-logo');
  if (slot === null) return;
  const { intro, micro, idle } = slot.dataset;
  const colours = { glowColour: cssToken(doc, win, '--brand'), bandColour: cssToken(doc, win, '--band') };
  const motion = rasterMotion({ intro, micro, idle }, colours);
  const style = doc.createElement('style');
  style.textContent = motion.css;
  doc.head.appendChild(style);
  slot.innerHTML = rasterMarkup(slot.dataset.src, slot.dataset.alt, { width: slot.dataset.width });
  const controls = startRasterMotion(slot.firstElementChild, { intro: true, idle: true, microMs: motion.microMs });
  const form = slot.closest('form');
  if (form !== null) wireLoginSubmit(form, controls, motion.microMs, win.matchMedia(REDUCED_QUERY).matches);
}
