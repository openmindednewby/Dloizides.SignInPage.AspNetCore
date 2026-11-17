export const SUBMIT_HOLD_CAP_MS = 300;

function playSafely(controls) {
  try {
    controls.micro();
  } catch (error) {
    console.error(error);
  }
}

export function wireLoginSubmit(form, controls, microMs, reducedMotion) {
  if (reducedMotion) return;
  const holdMs = Math.min(microMs, SUBMIT_HOLD_CAP_MS);
  let sending = false;
  form.addEventListener('submit', (event) => {
    if (sending) return;
    event.preventDefault();
    sending = true;
    setTimeout(() => form.submit(), holdMs);
    playSafely(controls);
  });
}
