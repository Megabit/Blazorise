import{u as O,c as N,e as w,L as Q,O as j,s as T,am as H,A as K,a1 as n,an as A,p as U,i as z,l as J,G as R,b as I,R as W,o as X,U as r}from"./vidstack-Bv1scj9I.js";import{u as _,L as Y,I as Z,$ as l,a as ee,S as te,M as ae,b as ne,c as se,d as le,e as re,f as oe,g as ie,h as ue,i as pe,j as ce,k as de,l as me,m as _e,n as ye,o as $e,p as be,q as fe,r as ve,s as ge,t as we,v as Te,w as he,x as ke,y as Me,z as xe}from"./vidstack-B51NjO8w.js";import{a as Pe,b as Ee}from"./vidstack-RD6foHms.js";import"./vidstack-CsZuoh6p.js";import"./vidstack-Bpbkek1D.js";import"./vidstack-Bdhe8WOd.js";import"./vidstack-C_AxqLKV.js";import"./vidstack-DRH_1tFW.js";import"./vidstack-BfBBPhXV.js";import"./vidstack-Bxv1Qnxe.js";import"../icons/index.js";const D=N();function o(){return O(D)}const Le={clickToPlay:!0,clickToFullscreen:!0,controls:["play-large","play","progress","current-time","mute+volume","captions","settings","pip","airplay","fullscreen"],customIcons:!1,displayDuration:!1,download:null,markers:null,invertTime:!0,thumbnails:null,toggleTime:!0,translations:null,seekTime:10,speed:[.5,.75,1,1.25,1.5,1.75,2,4]};class Ce extends Q{static props=Le;#e;onSetup(){this.#e=_(),j(D,{...this.$props,previewTime:T(0)})}}function Se(e,t){const{canAirPlay:a,canFullscreen:s,canPictureInPicture:u,controlsHidden:d,currentTime:$,fullscreen:m,hasCaptions:c,isAirPlayConnected:y,paused:b,pictureInPicture:g,playing:M,pointer:x,poster:P,textTrack:E,viewType:p,waiting:v}=t.$state;e.classList.add("plyr"),e.classList.add("plyr--full-ui");const S={"plyr--airplay-active":y,"plyr--airplay-supported":a,"plyr--fullscreen-active":m,"plyr--fullscreen-enabled":s,"plyr--hide-controls":d,"plyr--is-touch":()=>x()==="coarse","plyr--loading":v,"plyr--paused":b,"plyr--pip-active":g,"plyr--pip-enabled":u,"plyr--playing":M,"plyr__poster-enabled":P,"plyr--stopped":()=>b()&&$()===0,"plyr--captions-active":E,"plyr--captions-enabled":c},L=H();for(const f of Object.keys(S))L.add(w(()=>void e.classList.toggle(f,!!S[f]())));return L.add(w(()=>{const f=`plyr--${p()}`;return e.classList.add(f),()=>e.classList.remove(f)}),w(()=>{const{$provider:f}=t,C=f()?.type,B=`plyr--${Be(C)?"html5":C}`;return e.classList.toggle(B,!!C),()=>e.classList.remove(B)})),()=>L.empty()}function Be(e){return e==="audio"||e==="video"}class Ae extends Y{async loadIcons(){const t=(await import("./vidstack-Dge3KT8k.js")).icons,a={};for(const s of Object.keys(t))a[s]=Z({name:s,paths:t[s],viewBox:"0 0 18 18"});return a}}function h(e,t){return e()?.[t]??t}function Re(){return Ge()}function Ie(){const e=_(),{load:t}=e.$props,{canLoad:a}=e.$state;return K(()=>t()==="play"&&!a())()?[F(),G()]:[De(),Fe(),G(),Ve(),et(),tt()]}function F(){const e=_(),{translations:t}=o(),{title:a}=e.$state,s=l(()=>`${h(t,"Play")}, ${a()}`);return n`
    <media-play-button
      class="plyr__control plyr__control--overlaid"
      aria-label=${s}
      data-plyr="play"
    >
      <slot name="play-icon"></slot>
    </button>
  `}function De(){const{controls:e}=o();return l(()=>e().includes("play-large")?F():null)}function Fe(){const{thumbnails:e,previewTime:t}=o();return n`
    <media-thumbnail
      .src=${l(e)}
      class="plyr__preview-scrubbing"
      time=${l(()=>t())}
    ></media-thumbnail>
  `}function G(){const e=_(),{poster:t}=e.$state,a=l(()=>`background-image: url("${t()}");`);return n`<div class="plyr__poster" style=${a}></div>`}function Ge(){const e=new Set(["captions","pip","airplay","fullscreen"]),{controls:t}=o(),a=l(()=>t().filter(s=>!e.has(s)).map(V));return n`<div class="plyr__controls">${a}</div>`}function Ve(){const{controls:e}=o(),t=l(()=>e().map(V));return n`<div class="plyr__controls">${t}</div>`}function V(e){switch(e){case"airplay":return qe();case"captions":return Oe();case"current-time":return Ye();case"download":return Ze();case"duration":return q();case"fast-forward":return ze();case"fullscreen":return Ne();case"mute":case"volume":case"mute+volume":return We(e);case"pip":return je();case"play":return He();case"progress":return Je();case"restart":return Ke();case"rewind":return Ue();case"settings":return at();default:return null}}function qe(){const{translations:e}=o();return n`
    <media-airplay-button class="plyr__controls__item plyr__control" data-plyr="airplay">
      <slot name="airplay-icon"></slot>
      <span class="plyr__tooltip">${i(e,"AirPlay")}</span>
    </media-airplay-button>
  `}function Oe(){const{translations:e}=o(),t=i(e,"Disable captions"),a=i(e,"Enable captions");return n`
    <media-caption-button
      class="plyr__controls__item plyr__control"
      data-no-label
      data-plyr="captions"
    >
      <slot name="captions-on-icon" data-class="icon--pressed"></slot>
      <slot name="captions-off-icon" data-class="icon--not-pressed"></slot>
      <span class="label--pressed plyr__tooltip">${t}</span>
      <span class="label--not-pressed plyr__tooltip">${a}</span>
    </media-caption-button>
  `}function Ne(){const{translations:e}=o(),t=i(e,"Enter Fullscreen"),a=i(e,"Exit Fullscreen");return n`
    <media-fullscreen-button
      class="plyr__controls__item plyr__control"
      data-no-label
      data-plyr="fullscreen"
    >
      <slot name="enter-fullscreen-icon" data-class="icon--pressed"></slot>
      <slot name="exit-fullscreen-icon" data-class="icon--not-pressed"></slot>
      <span class="label--pressed plyr__tooltip">${a}</span>
      <span class="label--not-pressed plyr__tooltip">${t}</span>
    </media-fullscreen-button>
  `}function Qe(){const{translations:e}=o(),t=i(e,"Mute"),a=i(e,"Unmute");return n`
    <media-mute-button class="plyr__control" data-no-label data-plyr="mute">
      <slot name="muted-icon" data-class="icon--pressed"></slot>
      <slot name="volume-icon" data-class="icon--not-pressed"></slot>
      <span class="label--pressed plyr__tooltip">${a}</span>
      <span class="label--not-pressed plyr__tooltip">${t}</span>
    </media-mute-button>
  `}function je(){const{translations:e}=o(),t=i(e,"Enter PiP"),a=i(e,"Exit PiP");return n`
    <media-pip-button class="plyr__controls__item plyr__control" data-no-label data-plyr="pip">
      <slot name="pip-icon"></slot>
      <slot name="enter-pip-icon" data-class="icon--pressed"></slot>
      <slot name="exit-pip-icon" data-class="icon--not-pressed"></slot>
      <span class="label--pressed plyr__tooltip">${a}</span>
      <span class="label--not-pressed plyr__tooltip">${t}</span>
    </media-pip-button>
  `}function He(){const{translations:e}=o(),t=i(e,"Play"),a=i(e,"Pause");return n`
    <media-play-button class="plyr__controls__item plyr__control" data-no-label data-plyr="play">
      <slot name="pause-icon" data-class="icon--pressed"></slot>
      <slot name="play-icon" data-class="icon--not-pressed"></slot>
      <span class="label--pressed plyr__tooltip">${a}</span>
      <span class="label--not-pressed plyr__tooltip">${t}</span>
    </media-play-button>
  `}function Ke(){const{translations:e}=o(),{remote:t}=_(),a=i(e,"Restart");function s(u){R(u)&&!I(u)||t.seek(0,u)}return n`
    <button
      type="button"
      class="plyr__control"
      data-plyr="restart"
      @pointerup=${s}
      @keydown=${s}
    >
      <slot name="restart-icon"></slot>
      <span class="plyr__tooltip">${a}</span>
    </button>
  `}function Ue(){const{translations:e,seekTime:t}=o(),a=l(()=>`${h(e,"Rewind")} ${t()}s`),s=l(()=>-1*t());return n`
    <media-seek-button
      class="plyr__controls__item plyr__control"
      seconds=${s}
      data-no-label
      data-plyr="rewind"
    >
      <slot name="rewind-icon"></slot>
      <span class="plyr__tooltip">${a}</span>
    </media-seek-button>
  `}function ze(){const{translations:e,seekTime:t}=o(),a=l(()=>`${h(e,"Forward")} ${t()}s`),s=l(t);return n`
    <media-seek-button
      class="plyr__controls__item plyr__control"
      seconds=${s}
      data-no-label
      data-plyr="fast-forward"
    >
      <slot name="fast-forward-icon"></slot>
      <span class="plyr__tooltip">${a}</span>
    </media-seek-button>
  `}function Je(){let e=_(),{duration:t,viewType:a}=e.$state,{translations:s,markers:u,thumbnails:d,seekTime:$,previewTime:m}=o(),c=i(s,"Seek"),y=T(null),b=l(()=>{const p=y();return p?n`<span class="plyr__progress__marker-label">${A(p.label)}<br /></span>`:null});function g(p){m.set(p.detail)}function M(){y.set(this)}function x(){y.set(null)}function P(){const p=d(),v=l(()=>a()==="audio");return p?n`
          <media-slider-preview class="plyr__slider__preview" ?no-clamp=${v}>
            <media-slider-thumbnail .src=${p} class="plyr__slider__preview__thumbnail">
              <span class="plyr__slider__preview__time-container">
                ${b}
                <media-slider-value class="plyr__slider__preview__time"></media-slider-value>
              </span>
            </media-slider-thumbnail>
          </media-slider-preview>
        `:n`
          <span class="plyr__tooltip">
            ${b}
            <media-slider-value></media-slider-value>
          </span>
        `}function E(){const p=t();return Number.isFinite(p)?u()?.map(v=>n`
        <span
          class="plyr__progress__marker"
          @mouseenter=${M.bind(v)}
          @mouseleave=${x}
          style=${`left: ${v.time/p*100}%;`}
        ></span>
      `):null}return n`
    <div class="plyr__controls__item plyr__progress__container">
      <div class="plyr__progress">
        <media-time-slider
          class="plyr__slider"
          data-plyr="seek"
          pause-while-dragging
          key-step=${l($)}
          aria-label=${c}
          @media-seeking-request=${g}
        >
          <div class="plyr__slider__track"></div>
          <div class="plyr__slider__thumb"></div>
          <div class="plyr__slider__buffer"></div>
          ${l(P)}${l(E)}
        </media-time-slider>
      </div>
    </div>
  `}function We(e){return l(()=>{const t=e==="mute"||e==="mute+volume",a=e==="volume"||e==="mute+volume";return n`
      <div class="plyr__controls__item plyr__volume">
        ${[t?Qe():null,a?Xe():null]}
      </div>
    `})}function Xe(){const{translations:e}=o(),t=i(e,"Volume");return n`
    <media-volume-slider class="plyr__slider" data-plyr="volume" aria-label=${t}>
      <div class="plyr__slider__track"></div>
      <div class="plyr__slider__thumb"></div>
    </media-volume-slider>
  `}function Ye(){const e=_(),{translations:t,invertTime:a,toggleTime:s,displayDuration:u}=o(),d=T(U(a));function $(c){!s()||u()||R(c)&&!I(c)||d.set(y=>!y)}function m(){return l(()=>u()?q():null)}return l(()=>{const{streamType:c}=e.$state,y=i(t,"LIVE"),b=i(t,"Current time"),g=l(()=>!u()&&d());return c()==="live"||c()==="ll-live"?n`
          <media-live-button
            class="plyr__controls__item plyr__control plyr__live-button"
            data-plyr="live"
          >
            <span class="plyr__live-button__text">${y}</span>
          </media-live-button>
        `:n`
          <media-time
            type="current"
            class="plyr__controls__item plyr__time plyr__time--current"
            tabindex="0"
            role="timer"
            aria-label=${b}
            ?remainder=${g}
            @pointerup=${$}
            @keydown=${$}
          ></media-time>
          ${m()}
        `})}function q(){const{translations:e}=o(),t=i(e,"Duration");return n`
    <media-time
      type="duration"
      class="plyr__controls__item plyr__time plyr__time--duration"
      role="timer"
      tabindex="0"
      aria-label=${t}
    ></media-time>
  `}function Ze(){return l(()=>{const e=_(),{translations:t,download:a}=o(),{title:s,source:u}=e.$state,d=u(),$=a(),m=Pe({title:s(),src:d,download:$}),c=i(t,"Download");return z(m?.url)?n`
          <a
            class="plyr__controls__item plyr__control"
            href=${Ee(m.url,{download:m.name})}
            download=${m.name}
            target="_blank"
          >
            <slot name="download-icon" />
            <span class="plyr__tooltip">${c}</span>
          </a>
        `:null})}function et(){return l(()=>{const{clickToPlay:e,clickToFullscreen:t}=o();return[e()?n`
            <media-gesture
              class="plyr__gesture"
              event="pointerup"
              action="toggle:paused"
            ></media-gesture>
          `:null,t()?n`
            <media-gesture
              class="plyr__gesture"
              event="dblpointerup"
              action="toggle:fullscreen"
            ></media-gesture>
          `:null]})}function tt(){const e=_(),t=T(void 0),a=l(()=>A(t()?.text));return w(()=>{const s=e.$state.textTrack();if(!s)return;function u(){t.set(s?.activeCues[0])}return u(),J(s,"cue-change",u)}),n`
    <div class="plyr__captions" dir="auto">
      <span class="plyr__caption">${a}</span>
    </div>
  `}function at(){const{translations:e}=o(),t=i(e,"Settings");return n`
    <div class="plyr__controls__item plyr__menu">
      <media-menu>
        <media-menu-button class="plyr__control" data-plyr="settings">
          <slot name="settings-icon" />
          <span class="plyr__tooltip">${t}</span>
        </media-menu-button>
        <media-menu-items class="plyr__menu__container" placement="top end">
          <div><div>${[st(),it(),pt(),rt()]}</div></div>
        </media-menu-items>
      </media-menu>
    </div>
  `}function k({label:e,children:t}){const a=T(!1);return n`
    <media-menu @open=${()=>a.set(!0)} @close=${()=>a.set(!1)}>
      ${nt({label:e,open:a})}
      <media-menu-items>${t}</media-menu-items>
    </media-menu>
  `}function nt({open:e,label:t}){const{translations:a}=o(),s=l(()=>`plyr__control plyr__control--${e()?"back":"forward"}`);function u(){const d=i(a,"Go back to previous menu");return l(()=>e()?n`<span class="plyr__sr-only">${d}</span>`:null)}return n`
    <media-menu-button class=${s} data-plyr="settings">
      <span class="plyr__menu__label" aria-hidden=${dt(e)}>
        ${i(a,t)}
      </span>
      <span class="plyr__menu__value" data-part="hint"></span>
      ${u()}
    </media-menu-button>
  `}function st(){return k({label:"Audio",children:lt()})}function lt(){const{translations:e}=o();return n`
    <media-audio-radio-group empty-label=${i(e,"Default")}>
      <template>
        <media-radio class="plyr__control" data-plyr="audio">
          <span data-part="label"></span>
        </media-radio>
      </template>
    </media-audio-radio-group>
  `}function rt(){return k({label:"Speed",children:ot()})}function ot(){const{translations:e,speed:t}=o();return n`
    <media-speed-radio-group .rates=${t} normal-label=${i(e,"Normal")}>
      <template>
        <media-radio class="plyr__control" data-plyr="speed">
          <span data-part="label"></span>
        </media-radio>
      </template>
    </media-speed-radio-group>
  `}function it(){return k({label:"Captions",children:ut()})}function ut(){const{translations:e}=o();return n`
    <media-captions-radio-group off-label=${i(e,"Disabled")}>
      <template>
        <media-radio class="plyr__control" data-plyr="captions">
          <span data-part="label"></span>
        </media-radio>
      </template>
    </media-captions-radio-group>
  `}function pt(){return k({label:"Quality",children:ct()})}function ct(){const{translations:e}=o();return n`
    <media-quality-radio-group auto-label=${i(e,"Auto")}>
      <template>
        <media-radio class="plyr__control" data-plyr="quality">
          <span data-part="label"></span>
        </media-radio>
      </template>
    </media-quality-radio-group>
  `}function dt(e){return l(()=>e()?"true":"false")}function i(e,t){return l(()=>h(e,t))}class mt extends W(ee,Ce){static tagName="media-plyr-layout";#e;onSetup(){this.forwardKeepAlive=!1,this.#e=_()}onConnect(){this.#e.player.el?.setAttribute("data-layout","plyr"),X(()=>this.#e.player.el?.removeAttribute("data-layout")),Se(this,this.#e),w(()=>{this.$props.customIcons()?new te([this]).connect():new Ae([this]).connect()})}render(){return l(this.#t.bind(this))}#t(){const{viewType:t}=this.#e.$state;return t()==="audio"?Re():t()==="video"?Ie():null}}r(mt),r(ae),r(ne),r(se),r(le),r(re),r(oe),r(ie),r(ue),r(pe),r(ce),r(de),r(me),r(_e),r(ye),r($e),r(be),r(fe),r(ve),r(ge),r(we),r(Te),r(he),r(ke),r(Me),r(xe);
