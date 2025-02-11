import { V as VTTParser, a as VTTBlock, b as VTTCue } from './index.js';

const MILLISECOND_SEP_RE = /,/g, TIMESTAMP_SEP = "-->";
class SRTParser extends VTTParser {
  parse(line, lineCount) {
    if (line === "") {
      if (this.c) {
        this.l.push(this.c);
        this.h.onCue?.(this.c);
        this.c = null;
      }
      this.e = VTTBlock.None;
    } else if (this.e === VTTBlock.Cue) {
      this.c.text += (this.c.text ? "\n" : "") + line;
    } else if (line.includes(TIMESTAMP_SEP)) {
      const result = this.q(line, lineCount);
      if (result) {
        this.c = new VTTCue(result[0], result[1], result[2].join(" "));
        this.c.id = this.n;
        this.e = VTTBlock.Cue;
      }
    }
    this.n = line;
  }
  q(line, lineCount) {
    return super.q(line.replace(MILLISECOND_SEP_RE, "."), lineCount);
  }
}
function createSRTParser() {
  return new SRTParser();
}

export { SRTParser, createSRTParser as default };
