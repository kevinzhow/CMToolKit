using System;
using System.Collections.Generic;
using System.Linq;
using AppKit;
using CoreGraphics;
using Foundation;

namespace CMToolKit
{
    public class CMImage
    {
        public static bool SaveImageToFile(NSImage image, string filename, string folderPath)
        {

            string jpgFilename = System.IO.Path.Combine(folderPath, filename); // hardcoded filename, overwritten each time
            NSData imgData = image.AsTiff();
            NSError err = null;

            if (imgData.Save(jpgFilename, false, out err))
            {
                Console.WriteLine("saved as " + jpgFilename);
                return true;
            }
            else
            {
                Console.WriteLine("NOT saved as " + jpgFilename + " because" + err.LocalizedDescription);
                return false;
            }

        }

        public static NSImage MergeImages(List<NSImage> images)
        {
            var thumbnail_width = images.First().Size.Width;
            var imageHeight = images.First().Size.Height;
            var thumbnial_all_image = new NSImage(new CGSize(thumbnail_width * images.Count, imageHeight));

            thumbnial_all_image.LockFocus();
            var newImageRect = new CGRect(0, 0, 0, 0);
            newImageRect.Size = thumbnial_all_image.Size;

            int index = 0;
            foreach (var i in images)
            {
                i.DrawInRect(
                    new CGRect(thumbnail_width * index, 0, thumbnail_width, imageHeight),
                    new CGRect(0, 0, thumbnail_width, imageHeight),
                    NSCompositingOperation.SourceOver,
                    1);
                ++index;
            }
            thumbnial_all_image.UnlockFocus();

            return thumbnial_all_image;
        }

        public static NSImage ResizeImageToSize(NSImage sourceImage, CGSize destSize) {
            //Console.WriteLine("Original Size is {0}", sourceImage.Size);

            float renderWidth = (float)sourceImage.Size.Width;
            float renderHeight = (float)sourceImage.Size.Height;

            float cropWidth = (float)destSize.Width;
            float cropHeight = (float)destSize.Height;

            // Resize Width           
            var radio = renderWidth / cropWidth;

            cropHeight = cropHeight * radio;
            cropWidth = renderWidth;


            // Resize Height    
            if (cropHeight > renderHeight) {
                radio = renderHeight / cropHeight;
                cropWidth = cropWidth * radio;
                cropHeight = renderHeight;
            }

            //Console.WriteLine("Crop Size is {0}, {1}", cropWidth, cropHeight);
            var cropX = (renderWidth - cropWidth) / 2;
            var cropY = (renderHeight - cropHeight) / 2;

            var renderImage = new NSImage(destSize);

            renderImage.LockFocus();
            sourceImage.DrawInRect(
                    new CGRect(0, 0, destSize.Width, destSize.Height),
                new CGRect(cropX, cropY, cropWidth, cropHeight),
                    NSCompositingOperation.SourceOver,
                    1);
            renderImage.UnlockFocus();

            return renderImage;
        }

        public static NSImage CropImagesFromSizeToSize(NSImage sourceImage, CGSize sourceSize, CGSize destSize)
        {
            var originalWidth = sourceImage.Size.Width;
            var originalHeight = sourceImage.Size.Height;

            var corpWidth = (sourceSize.Height / destSize.Height) * destSize.Width;

            var numberOfFrame = originalWidth / sourceSize.Width;
            var thumbnial_all_image = new NSImage(new CGSize(destSize.Width * numberOfFrame, destSize.Height));

            thumbnial_all_image.LockFocus();
            var newImageRect = new CGRect(0, 0, 0, 0);
            newImageRect.Size = thumbnial_all_image.Size;


            for (var i = 0; i < numberOfFrame; i++)
            {
                sourceImage.DrawInRect(
                    new CGRect( (destSize.Width * i), 0, destSize.Width, destSize.Height),
                    new CGRect( (sourceSize.Width * i) + (sourceSize.Width/2) - (corpWidth/2) , 0, corpWidth, sourceSize.Height),
                    NSCompositingOperation.SourceOver,
                    1);
               
            }

            thumbnial_all_image.UnlockFocus();

            return thumbnial_all_image;
        }
    }

}