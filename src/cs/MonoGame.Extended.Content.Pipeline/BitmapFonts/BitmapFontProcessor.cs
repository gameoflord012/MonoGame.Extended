using System;
using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace MonoGame.Extended.Content.Pipeline.BitmapFonts
{
    [ContentProcessor(DisplayName = "BMFont Processor - MonoGame.Extended")]
    public class BitmapFontProcessor : ContentProcessor<BitmapFontFile, BitmapFontProcessorResult>
    {
        public override BitmapFontProcessorResult Process(BitmapFontFile bitmapFontFile, ContentProcessorContext context)
        {
            try
            {
                context.Logger.LogMessage("Processing BMFont");
                var result = new BitmapFontProcessorResult(bitmapFontFile);

                foreach (var fontPage in bitmapFontFile.Pages)
                {
                    var assetFileName = fontPage.File;
                    context.Logger.LogMessage("Expected texture asset: {0}", assetFileName);
                    var sourceDir = Path.GetDirectoryName(context.SourceIdentity.SourceFilename);
                    result.TextureAssets.Add(Path.Combine(sourceDir, assetFileName));
                }

                return result;
            }
            catch (Exception ex)
            {
                context.Logger.LogMessage("Error {0}", ex);
                throw;
            }
        }
    }
}
