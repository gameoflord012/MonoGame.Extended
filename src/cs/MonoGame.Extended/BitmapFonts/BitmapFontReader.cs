using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Content;
using MonoGame.Extended.TextureAtlases;
using MonoGame.Framework.Content.Pipeline.Builder;

namespace MonoGame.Extended.BitmapFonts
{
    public class BitmapFontReader : ContentTypeReader<BitmapFont>
    {
        protected override BitmapFont Read(ContentReader reader, BitmapFont existingInstance)
        {
            var textureAssetCount = reader.ReadInt32();
            var assets = new List<string>();

            for (var i = 0; i < textureAssetCount; i++)
            {
                var assetName = reader.ReadString();
                assets.Add(assetName);
            }

            var mgcontent = reader.AssetName + PipelineBuildEvent.Extension;
            var buildEvent = PipelineBuildEvent.Load(mgcontent);
            var sourceDir = Path.GetDirectoryName(buildEvent.SourceFile);

            var textures = assets
                .Select(textureName => reader.ContentManager.Load<Texture2D>(Path.Combine(sourceDir, textureName)))
                .ToArray();

            var lineHeight = reader.ReadInt32();
            var regionCount = reader.ReadInt32();
            var regions = new BitmapFontRegion[regionCount];

            for (var r = 0; r < regionCount; r++)
            {
                var character = reader.ReadInt32();
                var textureIndex = reader.ReadInt32();
                var x = reader.ReadInt32();
                var y = reader.ReadInt32();
                var width = reader.ReadInt32();
                var height = reader.ReadInt32();
                var xOffset = reader.ReadInt32();
                var yOffset = reader.ReadInt32();
                var xAdvance = reader.ReadInt32();
                var textureRegion = new TextureRegion2D(textures[textureIndex], x, y, width, height);
                regions[r] = new BitmapFontRegion(textureRegion, character, xOffset, yOffset, xAdvance);
            }

            var characterMap = regions.ToDictionary(r => r.Character);
            var kerningsCount = reader.ReadInt32();

            for (var k = 0; k < kerningsCount; k++)
            {
                var first = reader.ReadInt32();
                var second = reader.ReadInt32();
                var amount = reader.ReadInt32();

                // Find region
                if (!characterMap.TryGetValue(first, out var region))
                    continue;

                region.Kernings[second] = amount;
            }

            return new BitmapFont(reader.AssetName, regions, lineHeight);
        }
    }
}
