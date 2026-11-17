import { wireDemoRows } from './demo-rows.js';

wireDemoRows(document);

const slot = document.getElementById('login-logo');
if (slot !== null && slot.dataset.intro !== undefined) {
  import('./login-motion.js')
    .then((module) => module.mountLoginLogo(document, window))
    .catch((error) => console.error(error));
}
