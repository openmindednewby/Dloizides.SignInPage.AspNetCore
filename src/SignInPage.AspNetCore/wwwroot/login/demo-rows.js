export function wireDemoRows(doc) {
  const form = doc.querySelector('form.card');
  doc.querySelectorAll('.demo-row').forEach((row) => {
    row.addEventListener('click', () => {
      const email = doc.getElementById('email');
      const password = doc.getElementById('password');
      if (email !== null) email.value = row.getAttribute('data-email');
      if (password !== null) password.value = row.getAttribute('data-password');
      if (form === null) return;
      if (typeof form.requestSubmit === 'function') form.requestSubmit();
      else form.submit();
    });
  });
}
