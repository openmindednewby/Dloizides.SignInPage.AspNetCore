import { test } from 'node:test';
import assert from 'node:assert/strict';
import { readFileSync } from 'node:fs';
import { rasterMotion } from '../../src/SignInPage.AspNetCore/wwwroot/login/vendor/logo-kit-runtime.js';

const KNOWN_GAP = new Set([
  ...['draw', 'flood', 'stagger-rise', 'scale-pop', 'orbit', 'wipe', 'morph-from-circle', 'sweep', 'pulse', 'fly-in',
    'spin-assemble', 'drop-bounce', 'flip', 'slam-shake'].map((name) => `intro:${name}`),
  'micro:spread', 'micro:sweep',
  ...['rim-glint', 'faces-part', 'breath', 'orbit-spark'].map((name) => `idle:${name}`),
]);

const schema = JSON.parse(readFileSync(new URL('../../src/SignInPage.AspNetCore/schema/sign-in-config.schema.json', import.meta.url)));
const motion = schema.$defs.motion.properties;
const steps = [
  ['intro', motion.intro.properties.preset.enum],
  ['micro', motion.micro.properties.preset.enum],
  ['idle', motion.idle.properties.recipe.enum],
];

for (const [step, names] of steps) {
  for (const name of names) {
    const todo = KNOWN_GAP.has(`${step}:${name}`) ? 'MODC-9: schema preset has no logo-kit 0.6.0 raster spec; owner decision pending' : false;
    test(`rasterMotion accepts the schema ${step} preset ${name}`, { todo }, () => {
      const result = rasterMotion({ [step]: name });

      assert.deepEqual(result.unknown, []);
    });
  }
}
