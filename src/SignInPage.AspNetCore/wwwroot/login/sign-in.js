import { wireDemoRows } from './demo-rows.js';
import { wireFieldValidation } from './field-validation.js';

const form = document.querySelector('form.card');
if (form !== null) wireFieldValidation(form, document);

wireDemoRows(document);

const slot = document.getElementById('login-logo');
if (slot !== null && slot.dataset.intro !== undefined) {
  import('./login-motion.js')
    .then((module) => module.mountLoginLogo(document, window))
    .catch((error) => console.error(error));
}
