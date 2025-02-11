import { P as ParseError, c as ParseErrorCode } from './index.js';

const ParseErrorBuilder = {
  r() {
    return new ParseError({
      code: ParseErrorCode.BadSignature,
      reason: "missing WEBVTT file header",
      line: 1
    });
  },
  s(startTime, line) {
    return new ParseError({
      code: ParseErrorCode.BadTimestamp,
      reason: `cue start timestamp \`${startTime}\` is invalid on line ${line}`,
      line
    });
  },
  t(endTime, line) {
    return new ParseError({
      code: ParseErrorCode.BadTimestamp,
      reason: `cue end timestamp \`${endTime}\` is invalid on line ${line}`,
      line
    });
  },
  u(startTime, endTime, line) {
    return new ParseError({
      code: ParseErrorCode.BadTimestamp,
      reason: `cue end timestamp \`${endTime}\` is greater than start \`${startTime}\` on line ${line}`,
      line
    });
  },
  y(name, value, line) {
    return new ParseError({
      code: ParseErrorCode.BadSettingValue,
      reason: `invalid value for cue setting \`${name}\` on line ${line} (value: ${value})`,
      line
    });
  },
  x(name, value, line) {
    return new ParseError({
      code: ParseErrorCode.UnknownSetting,
      reason: `unknown cue setting \`${name}\` on line ${line} (value: ${value})`,
      line
    });
  },
  w(name, value, line) {
    return new ParseError({
      code: ParseErrorCode.BadSettingValue,
      reason: `invalid value for region setting \`${name}\` on line ${line} (value: ${value})`,
      line
    });
  },
  v(name, value, line) {
    return new ParseError({
      code: ParseErrorCode.UnknownSetting,
      reason: `unknown region setting \`${name}\` on line ${line} (value: ${value})`,
      line
    });
  },
  // SSA-specific errors
  T(type, line) {
    return new ParseError({
      code: ParseErrorCode.BadFormat,
      reason: `format missing for \`${type}\` block on line ${line}`,
      line
    });
  }
};

export { ParseErrorBuilder };
