import { test, mock } from 'node:test';
import assert from 'node:assert/strict';
import { wireLoginSubmit } from '../../src/SignInPage.AspNetCore/wwwroot/login/login-submit.js';

function fakeForm() {
  const form = { listeners: [], submits: 0 };
  form.addEventListener = (type, listener) => form.listeners.push({ type, listener });
  form.submit = () => { form.submits += 1; };
  return form;
}

function submitEvent(form) {
  const event = { defaultPrevented: false, preventDefault() { this.defaultPrevented = true; } };
  form.listeners.filter((entry) => entry.type === 'submit').forEach((entry) => entry.listener(event));
  return event;
}

test('wireLoginSubmit with a 2000 ms glint, submits no later than 300 ms', (t) => {
  t.mock.timers.enable({ apis: ['setTimeout'] });
  const form = fakeForm();
  wireLoginSubmit(form, { micro() {} }, 2000, false);

  const event = submitEvent(form);

  assert.equal(event.defaultPrevented, true);
  t.mock.timers.tick(299);
  assert.equal(form.submits, 0);
  t.mock.timers.tick(1);
  assert.equal(form.submits, 1);
});

test('wireLoginSubmit when micro throws, still submits the form', (t) => {
  t.mock.timers.enable({ apis: ['setTimeout'] });
  const logged = mock.method(console, 'error', () => {});
  const form = fakeForm();
  wireLoginSubmit(form, { micro() { throw new Error('glint failed'); } }, 200, false);

  submitEvent(form);

  t.mock.timers.tick(200);
  assert.equal(form.submits, 1);
  assert.equal(logged.mock.callCount(), 1);
  logged.mock.restore();
});

test('wireLoginSubmit under reduced motion, lets the native submit go at once', () => {
  const form = fakeForm();
  wireLoginSubmit(form, { micro() {} }, 200, true);

  const event = submitEvent(form);

  assert.equal(event.defaultPrevented, false);
  assert.equal(form.listeners.length, 0);
});
