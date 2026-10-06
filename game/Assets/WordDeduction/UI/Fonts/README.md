# UI fonts

Inter is the primary UI font (existing bundled license). Player names use Unity ATG and lazy installed script fallbacks.

Noto Color Emoji is bundled because Android 16's installed COLR-v1-only font returned blank emoji in the actual Unity build. This source has CBDT/CBLC bitmaps and GSUB shaping, used through a dynamic COLOR FontAsset before installed emoji fallback. Actual sequence rendering still requires the Android release gate.

- Upstream: Google Noto Emoji, release `v2026-09-24-unicode18_0`.
- Immutable source: https://raw.githubusercontent.com/googlefonts/noto-emoji/e20cbc2bbec1926686be9f9bee7d1d2cfa1fea0e/2D/fonts/NotoColorEmoji.ttf
- SHA-256: `15671215ab769fdc7162a045d56fd7d7e477c51b04e6b3c761d914d8fdd6cc44` (10,730,124 bytes).
- License: SIL Open Font License 1.1, unmodified full notice shipped at `../Resources/NotoColorEmoji-LICENSE.txt`.
- Font tables verified from the downloaded upstream file: CBDT, CBLC, GSUB; no COLR dependency.

The pin is deliberate. Do not replace it with an arbitrary installed Noto face or assume that a successful FontAsset load proves emoji rendering.