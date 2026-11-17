"use strict";
const test = require("node:test");
const assert = require("node:assert/strict");
const { directionFromAngles, raySphereDistance } = require("../src/combat");

test("yaw 90 points along positive X", () => {
  const d = directionFromAngles(90, 0);
  assert.ok(Math.abs(d.x - 1) < 1e-9);
  assert.ok(Math.abs(d.z) < 1e-9);
});
test("ray hits a target directly ahead", () => {
  const d = directionFromAngles(90, 0);
  const distance = raySphereDistance({ x: -500, y: 2, z: 0 }, d, { x: 500, y: 2, z: 0 }, 1.25, 2000);
  assert.ok(distance !== null && Math.abs(distance - 998.75) < 1e-6);
});
test("ray misses a target outside its radius", () => {
  const d = directionFromAngles(90, 0);
  assert.equal(raySphereDistance({ x: 0, y: 0, z: 0 }, d, { x: 10, y: 10, z: 0 }, 1, 2000), null);
});
test("ray rejects targets beyond maximum range", () => {
  const d = directionFromAngles(90, 0);
  assert.equal(raySphereDistance({ x: 0, y: 0, z: 0 }, d, { x: 2100, y: 0, z: 0 }, 1, 2000), null);
});
