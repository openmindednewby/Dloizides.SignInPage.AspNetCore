import { test } from 'node:test';
import assert from 'node:assert/strict';
import { wireDemoRows } from '../../src/SignInPage.AspNetCore/wwwroot/login/demo-rows.js';

function fakeRow(email, password) {
  const row = { listeners: {} };
  row.getAttribute = (name) => ({ 'data-email': email, 'data-password': password })[name];
  row.addEventListener = (type, listener) => { row.listeners[type] = listener; };
  return row;
}

function fakeDocument(rows, form) {
  const fields = { email: { value: '' }, password: { value: '' } };
  return {
    fields,
    querySelector: (selector) => (selector === 'form.card' ? form : null),
    querySelectorAll: (selector) => (selector === '.demo-row' ? rows : []),
    getElementById: (id) => fields[id] ?? null,
  };
}

test('wireDemoRows when a row is clicked, fills both fields and requests submit', () => {
  const row = fakeRow('analyst@acme.test', 's3cret');
  const form = { requested: 0, requestSubmit() { this.requested += 1; } };
  const doc = fakeDocument([row], form);
  wireDemoRows(doc);

  row.listeners.click();

  assert.equal(doc.fields.email.value, 'analyst@acme.test');
  assert.equal(doc.fields.password.value, 's3cret');
  assert.equal(form.requested, 1);
});

test('wireDemoRows without requestSubmit, falls back to submit', () => {
  const row = fakeRow('ops@acme.test', 'pw');
  const form = { submitted: 0, submit() { this.submitted += 1; } };
  const doc = fakeDocument([row], form);
  wireDemoRows(doc);

  row.listeners.click();

  assert.equal(form.submitted, 1);
});

test('wireDemoRows without a form, fills the fields and does not throw', () => {
  const row = fakeRow('ops@acme.test', 'pw');
  const doc = fakeDocument([row], null);
  wireDemoRows(doc);

  row.listeners.click();

  assert.equal(doc.fields.password.value, 'pw');
});
