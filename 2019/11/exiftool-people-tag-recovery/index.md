
### Need to recover your Windows Photo Gallery metadata so that you can easily sort your photos in current editing software? Find out why you need ExifTool.

Windows Photo Gallery’s People Tags feature recognized facial regions within an image and allowed users to tag it with a name. By doing so, Photo Gallery saved metadata to the image so they could be sorted by the people tagged. While the feature was useful, Microsoft announced they would no longer support the product or make it available for download after January 10, 2017. When these tagged images are moved into another modern photo editing software such as Adobe Lightroom Classic, the metadata may no longer be recognized. Before retagging the affected photos, consider the following solutions.

### Converting the Tags - A Partial Solution

Thanks to Jose Oliver-Didier’s [article](https://jmoliver.wordpress.com/2017/02/19/converting-and-exporting-windows-photo-gallery-people-tags/) “Converting and Exporting Windows Photo Gallery People Tags” and Phil Harvey’s [ExifTool](https://www.sno.phy.queensu.ca/~phil/exiftool/), a part of the solution was quickly discovered. The instructions provided look at the Region Info MP tags and generate new Region Info tags that follow the Metadata Working Group (MWG) standards. This new tag is recognized by Lightroom Classic, so all those photos don’t need to be retagged.

However, there is a catch.

#### The Orientation Issue

The face tag does not line up on photos where the camera was rotated. Either ExifTool or Microsoft seems to have a problem with images tagged with “Orientation: Rotate 270CW” and “Orientation: Rotate 90CW”. In either case, running the images through the tool with the Convert Region config file causes the regions to rotate 180 degrees. The most likely cause of this error is due to discrepancies between origin software and the current system attempting to utilize this orientation tag.

Here is an example photo to demonstrate this issue, viewed first with Windows Photo Gallery (before using the tool) and then with Lightroom Classic (after using the tool):

![Original photo with correct people tag.](https://raw.githubusercontent.com/worseTyler/MarkdownBlogs/main/2019/11/exiftool-people-tag-recovery/images/GrantW-Blog-GalleryImageWithMP.png)

This photo shows the original photo with the People Tag lining up correctly.

![Photo after using the tool. People tag is in the wrong location.](https://raw.githubusercontent.com/worseTyler/MarkdownBlogs/main/2019/11/exiftool-people-tag-recovery/images/GrantW-Blog-LighroomImageWithMWG.png)

This photo shows the photo after it had been altered with the tool. The People Tag, while still there, is now in the wrong position on the photo.

### A Full Solution Using ExifTool

ExifTool contributor, StarGeek, solved the rotation problem in this [forum post](https://exiftool.org/forum/index.php/topic,6354.msg32157.html#msg32157).

In order to put all of this together, we need three things.

#### **1\. Install ExifTool:**

[Click here](https://sno.phy.queensu.ca/~phil/exiftool/install.html) for the installation instructions.

The commands provided assume the tool was installed using stand-alone executable instructions. If another method was chosen, “.pl” may need to be added after “exiftool” in the commands.

#### **2\. Configuration files (using stand-alone\*):**

- Click here for the [convert regions file](https://sourceforge.net/p/exiftool/code/ci/master/tree/config_files/convert_regions.config).
- Click here for the [rotate regions file](https://sourceforge.net/p/exiftool/code/ci/master/tree/config_files/rotate_regions.config).

Place these files in the root directory of the images you want to run the tool on.

_\*Note: If you installed the full distribution_, _you shouldn't have to move the config files._

#### **3\. The commands:**

Open the command prompt and navigate to the root directory of the jpgs you want to run the tool on.

Run the rotation commands before and after the conversion command. Only running the rotation commands once will conditionally move the Region Info MP regions into the correct place for the new Region Info tags. Running the rotation commands again will move them back so that they are not ruined for other software that can read the Region Info MP tags. The conversion command will generate a Region Info tag on all jpg’s based on the existing MP Tags.

_\*Note: Make back-ups manually before starting this process to avoid messing up images. These commands work recursively starting with the current directory where you are running the tool. The "overwrite\_original" flag stops ExifTool_ _from making a copy of the original files before mutating them. Generated copies aren’t entirely helpful in our case since we are running the tool several times, and we don’t want to end up with malformed copies when we’re finished._

**Rotation Commands:**

exiftool -config Rotate\_Regions.config -if "$Orientation eq 'Rotate 270 CW'" "-RegionInfoMP<RotateMPRegionCW180" -r -ext jpg . -overwrite\_original

exiftool -config Rotate\_Regions.config -if "$Orientation eq 'Rotate 90 CW'" "-RegionInfoMP<RotateMPRegionCW180" -r -ext jpg . -overwrite\_original

**Conversion Command:**

exiftool -config convert\_regions.config "-regioninfo<myregion" -r -ext jpg . -overwrite\_original

Now all photos with existing people information can be understood by Adobe Lightroom Classic and other software that follows the MWG standard, while leaving the original Region Info MP tags intact.

### Consider other operating systems

ExifTool is a very flexible, powerful tool. Even though this solution only tested for jpgs in a windows environment, it is entirely possible to use it on other operating systems with all kinds of file types.

### Interested in ExifTool?

I highly recommend making an account on [their forums](https://exiftool.org/forum/index.php?PHPSESSID=71d5ee2bfeb43521e7bcdc118f7bf0b3&). The creator of the tool is on there regularly and is happy to help!

### Did this solution work for you?

Did you find a better solution? Let me know what you think in the comments, or tell me about your experience!
