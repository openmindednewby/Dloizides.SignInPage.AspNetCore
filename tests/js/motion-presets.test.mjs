import { test } from 'node:test';
import assert from 'node:assert/strict';
import { readFileSync } from 'node:fs';
import { rasterMotion } from '../../src/SignInPage.AspNetCore/wwwroot/login/vendor/logo-kit-runtime.js';

const schema = JSON.parse(readFileSync(new URL('../../src/SignInPage.AspNetCore/schema/sign-in-config.schema.json', import.meta.url)));
const motion = schema.$defs.motion.properties;
const steps = [
  ['intro', motion.intro.properties.preset.enum],
  ['micro', motion.micro.properties.preset.enum],
  ['idle', motion.idle.properties.recipe.enum],
];

for (const [step, names] of steps) {
  for (const name of names) {
    test(`rasterMotion accepts the schema ${step} preset ${name}`, () => {
      const result = rasterMotion({ [step]: name });

      assert.deepEqual(result.unknown, []);
    });
  }
}

for (const [step, name] of [['intro', 'stagger-rise'], ['micro', 'sweep'], ['idle', 'breath']]) {
  test(`rasterMotion with the retired ${step} name ${name}, reports it unknown`, () => {
    const result = rasterMotion({ [step]: name });

    assert.deepEqual(result.unknown, [name]);
  });
}
