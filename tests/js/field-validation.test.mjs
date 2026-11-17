import { test } from 'node:test';
import assert from 'node:assert/strict';
import { fieldMessage, wireFieldValidation } from '../../src/SignInPage.AspNetCore/wwwroot/login/field-validation.js';

function fakeInput(value, dataset) {
  return { value, dataset, id: 'password', attributes: {}, setAttribute(name, v) { this.attributes[name] = v; }, removeAttribute(name) { delete this.attributes[name]; }, focus() { this.focused = true; }, addEventListener() {} };
}

test('fieldMessage with an empty required field, returns the required message', () => {
  const input = fakeInput('', { field: 'password', required: 'true', msgRequired: 'Password is required' });

  const message = fieldMessage(input);

  assert.equal(message, 'Password is required');
});

test('fieldMessage with a plain text in an email field, returns the email message', () => {
  const input = fakeInput('not-an-email', { field: 'username', kind: 'email', msgEmail: 'Enter a valid email address' });

  const message = fieldMessage(input);

  assert.equal(message, 'Enter a valid email address');
});

test('fieldMessage with a value under the minimum, returns the too-short message', () => {
  const input = fakeInput('short', { field: 'password', min: '8', msgMin: 'Password needs at least 8 characters' });

  const message = fieldMessage(input);

  assert.equal(message, 'Password needs at least 8 characters');
});

test('fieldMessage with a pattern mismatch, returns the pattern message', () => {
  const input = fakeInput('nodigits', { field: 'password', pattern: '^(?=.*[0-9]).+$', msgPattern: 'Password needs a digit' });

  const message = fieldMessage(input);

  assert.equal(message, 'Password needs a digit');
});

test('wireFieldValidation with an invalid field, blocks the submit and shows the message inline', () => {
  const input = fakeInput('', { field: 'password', required: 'true', msgRequired: 'Password is required' });
  const error = { textContent: '' };
  const listeners = [];
  const form = { querySelectorAll: () => [input], addEventListener: (type, listener, options) => listeners.push({ type, listener, options }) };
  const doc = { getElementById: (id) => (id === 'password-error' ? error : null) };
  wireFieldValidation(form, doc);
  const event = { prevented: false, stopped: false, preventDefault() { this.prevented = true; }, stopImmediatePropagation() { this.stopped = true; } };

  listeners.filter((entry) => entry.type === 'submit').forEach((entry) => entry.listener(event));

  assert.equal(event.prevented, true);
  assert.equal(event.stopped, true);
  assert.equal(error.textContent, 'Password is required');
  assert.equal(input.attributes['aria-invalid'], 'true');
});
