# Changelog

## 2.0.0
- Breaking: `SignInPageModel(SignInPageShell, SignInConfig, SignInForm)`; theme, copy, motion and features come from a validated `SignInConfig`; `SignInTheme.Colors` adds the `success` token; demo copy uses `{label}`; the old `SignInMotion(Intro, Micro, Idle)` record is replaced by config motion.
- `SignInConfigParser.Parse/Load` and `services.AddSignInConfig(path)`: the embedded @dloizides/sign-in-config 1.0.0 schema, colour and 4.5:1 contrast checks, pattern and range checks, remember-me session cap; an invalid file fails at startup with every field path.
- Remember me: `RememberMe.Properties(config, checked, now, sessionMaxHours)`.
- Forgot password: `IPasswordResetHost`, `PasswordResetService`, `PasswordResetFlow`, `services.AddSignInPasswordReset(shell)`, `app.MapSignInPasswordReset(publicOrigin)`; single-use expiring hashed tokens, same answer for unknown emails.
- Field validation inline (field-validation.js, aria-live) and on the server (`SignInFieldValidator`).
- Inputs use autocomplete username / current-password; protocol-relative (//host) URLs are rejected.

## 1.0.0
- First release: `SignInPage.Render(SignInPageModel)`, theme tokens, demo accounts, logo slot with logo-kit 0.6.0 raster motion, 44px demo toggle.
