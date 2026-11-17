const EMAIL = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

function matches(pattern, value) {
  try {
    return new RegExp(pattern, 'u').test(value);
  } catch {
    return true;
  }
}

export function fieldMessage(input) {
  const rule = input.dataset;
  const value = input.value ?? '';
  if (value === '') return rule.required === 'true' ? rule.msgRequired : '';
  if ((rule.kind === 'email' || input.type === 'email') && !EMAIL.test(value)) return rule.msgEmail;
  const length = [...value].length;
  if (rule.min !== undefined && length < Number(rule.min)) return rule.msgMin;
  if (rule.max !== undefined && length > Number(rule.max)) return rule.msgMax;
  if (rule.pattern !== undefined && !matches(rule.pattern, value)) return rule.msgPattern;
  return '';
}

export function showFieldMessage(input, doc) {
  const message = fieldMessage(input) ?? '';
  const slot = doc.getElementById(`${input.id}-error`);
  if (slot !== null) slot.textContent = message;
  if (message === '') input.removeAttribute('aria-invalid');
  else input.setAttribute('aria-invalid', 'true');
  return message;
}

export function wireFieldValidation(form, doc) {
  const inputs = [...form.querySelectorAll('[data-field]')];
  inputs.forEach((input) => input.addEventListener('blur', () => showFieldMessage(input, doc)));
  form.addEventListener('submit', (event) => {
    const invalid = inputs.filter((input) => showFieldMessage(input, doc) !== '');
    if (invalid.length === 0) return;
    event.preventDefault();
    event.stopImmediatePropagation();
    invalid[0].focus();
  });
}
