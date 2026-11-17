"use strict";

function directionFromAngles(yawDegrees, pitchDegrees) {
  const yaw = yawDegrees * Math.PI / 180;
  const pitch = pitchDegrees * Math.PI / 180;
  const cp = Math.cos(pitch);
  return {
    x: Math.sin(yaw) * cp,
    y: Math.sin(pitch),
    z: Math.cos(yaw) * cp
  };
}

// Returns the nearest positive ray/sphere intersection, or null.
function raySphereDistance(origin, direction, center, radius, maxDistance) {
  const ox = origin.x - center.x;
  const oy = origin.y - center.y;
  const oz = origin.z - center.z;
  const b = ox * direction.x + oy * direction.y + oz * direction.z;
  const c = ox * ox + oy * oy + oz * oz - radius * radius;
  const discriminant = b * b - c;
  if (discriminant < 0) return null;
  const root = Math.sqrt(discriminant);
  const near = -b - root;
  const far = -b + root;
  const hit = near >= 0 ? near : far;
  return hit >= 0 && hit <= maxDistance ? hit : null;
}

module.exports = { directionFromAngles, raySphereDistance };
