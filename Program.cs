using System;
using System.Drawing;

namespace ArtSiggySystem {
	class Program {

		// This is just as an emergency countermeasure. If you really want a signature that's the entire bee movie script or something, set this to int.MaxValue
		public static readonly int MAX_SIGNATURE_LENGTH = 100;

		static void Main(string[] args) {

			//////////////
			// GET DATA //
			//////////////

			// Get user's signature.
			Console.ForegroundColor = ConsoleColor.Green;
			Console.Write("Enter the text that you would like your signature to say\n> ");
			Console.ForegroundColor = ConsoleColor.Cyan;
			string sigText = Console.ReadLine();
			Console.ForegroundColor = ConsoleColor.Green;

			// Get the width of their signature in pixels, if desired.
			Console.WriteLine("Enter the maximum width of the pixels. If the text is bigger than this, it will move down a row.");
			Console.ForegroundColor = ConsoleColor.DarkGreen;
			Console.WriteLine("(Leave this empty or enter 0 to make it one line only.)");
			Console.ForegroundColor = ConsoleColor.Green;
			Console.Write("> ");
			Console.ForegroundColor = ConsoleColor.Cyan;
			string sigWidth = Console.ReadLine();
			Console.ForegroundColor = ConsoleColor.Green;

			// And lastly, get the name of the file.
			Console.Write("What would you like to name your signature file (.png will be added automatically)?\n*** Do not put a directory here. Just the name. ***\n> ");
			Console.ForegroundColor = ConsoleColor.Cyan;
			string sigName = Console.ReadLine();
			Console.ForegroundColor = ConsoleColor.Green;

			// Display some message to let the user know it's doing something.
			Console.WriteLine();
			Console.WriteLine("Working...");


			///////////////////
			// SANITY CHECKS //
			///////////////////

			// Make sure the text is within the width and isn't nothing.
			if (sigText == "") {
				// They put nothing in for their signature.
				Console.ForegroundColor = ConsoleColor.DarkYellow;
				Console.WriteLine("You didn't enter any text for your signature! The program can't do anything without this information.\nPress any key to quit.");
				Console.ReadKey();
				return; // Exit this function.
			}

			if (sigText.Length > MAX_SIGNATURE_LENGTH) {
				// Text is longer than the maximum. Warn and cut the text.
				Console.ForegroundColor = ConsoleColor.DarkYellow;
				Console.WriteLine("WARNING: The text is too long (Over {0} characters)! It has been cut to match this length.", MAX_SIGNATURE_LENGTH);
				sigText = sigText.Substring(0, MAX_SIGNATURE_LENGTH);
			}

			// Sanity check the width value.
			int width = int.MaxValue; // Default it to a really huge number.
			if (sigWidth != "" && !int.TryParse(sigWidth, out width)) {
				// If the text isn't nothing (they didn't just press enter) BUT the text can't be turned into a number...
				Console.ForegroundColor = ConsoleColor.DarkYellow;
				Console.WriteLine("WARNING: Could not turn the value \"{0}\" into a number! Max width will be set to show your text on one line.", sigWidth);
				Console.ForegroundColor = ConsoleColor.Green;
				width = int.MaxValue;
			}
			if (width < 0) {
				// If width is a negative number...
				Console.ForegroundColor = ConsoleColor.DarkYellow;
				Console.WriteLine("WARNING: Width is less than 0! Max width will be set to show your text on one line.");
				Console.ForegroundColor = ConsoleColor.Green;
				width = int.MaxValue;
			}
			if (width == 0) {
				// If width is 0, default it to the max value.
				width = int.MaxValue;
			}

			// Sanity check the signature name.
			if (sigName == "") {
				Console.ForegroundColor = ConsoleColor.DarkGreen;
				Console.WriteLine("Signature file name is empty! Defaulting to \"signature.png\"");
				Console.ForegroundColor = ConsoleColor.Green;
				sigName = "signature";
			}


			// Divide the length of the text by 3. This is because we can fit 3 letters into one RGB pixel.
			int textLen = (int)Math.Ceiling(sigText.Length / 3.0);
			// Set the width of the bitmap to whichever is less - the length of the text or the width value (if width is less, we want more lines)
			int bmWidth = Math.Min(textLen, width);
			// Set the height of the bitmap to the ceiling of text length divided by width. Ceiling will round a number up so long as it has *any* decimal. 5.001 will round up to 6, for instance.
			int bmHeight = (int)Math.Ceiling(textLen / (double)bmWidth);

			// Dummy values for the current pixel location we're writing.
			int x = 0;
			int y = 0;

			// Tell the user how big their image will be and make a new image.
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("\nImage size will be {0} px by {1} px", bmWidth, bmHeight);
			Bitmap bmp = new Bitmap(bmWidth, bmHeight);

			// Set up a byte array that's 3 bytes long to store R, G, and B.
			byte[] clrStorage = new byte[3];
			// Set up an index so we know which position to write in ^ (first, second, or third)
			byte idx = 0;
			foreach (char c in sigText) {
				// Go through every character in the signature...
				// Convert the character to its ASCII byte value and put it into the color storage array.
				clrStorage[idx] = (byte)c;
				idx++; // Advance the index so that next time we run ^, it writes to the next available spot.

				// If the index is 3 after advancing it above, we need to write the data since the array is full.
				// (If you are new to programming like this and are reading these comments to find out how it works, arrays start at the "0th" index. 0, 1, and 2 are valid indexes, 3 is too big.)
				if (idx == 3) {
					byte r = clrStorage[0];
					byte g = clrStorage[1];
					byte b = clrStorage[2];
					// Report that we are writing a pixel value. Show the color. This function is at the very bottom.
					ReportWritingColor(r, g, b);

					// Write the pixel at our current position.
					bmp.SetPixel(x, y, Color.FromArgb(r, g, b));

					// Advance the current X pixel by 1 (so if we wrote at [0, 0], next time we will write at [1, 0]
					x++;
					if (x == bmWidth) {
						// But wait! If the x position is more than the width of the image...
						// Set x to 0 so we are at the very left again, then advance y by 1 to move down a row.
						x = 0;
						y++;
					}
					// Reset the index back to 0.
					idx = 0;
					// Reset the R, G, and B values in color storage to be 0, 0, 0.
					clrStorage[0] = 0;
					clrStorage[1] = 0;
					clrStorage[2] = 0;
				}
			}
			// Catch case!
			if (idx != 0) {
				// If idx isn't 0, then that means the length of our text wasn't a multiple of 3.
				// This means that there's still some stuff left over in color storage. Maybe we only put 2 colors in because that's all the data we had left.
				// Write that incomplete data now. The empty values will be 0.
				byte r = clrStorage[0];
				byte g = clrStorage[1];
				byte b = clrStorage[2];
				ReportWritingColor(r, g, b);
				bmp.SetPixel(x, y, Color.FromArgb(r, g, b));
			}

			// Save the image.
			bmp.Save(".\\" + sigName + ".png");
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("Done! Press any key to quit.");
			Console.ReadKey(true);
		}

		public static void ReportWritingColor(byte r, byte g, byte b) {
			// Write the "Writing RGB[r, g, b]:" part of the text with some fancy formatting.
			Console.ForegroundColor = ConsoleColor.Gray;
			Console.Write("Writing RGB[");
			Console.ForegroundColor = ConsoleColor.Red;
			Console.Write(r);
			Console.ForegroundColor = ConsoleColor.Gray;
			Console.Write(", ");
			Console.ForegroundColor = ConsoleColor.Green;
			Console.Write(g);
			Console.ForegroundColor = ConsoleColor.Gray;
			Console.Write(", ");
			Console.ForegroundColor = ConsoleColor.Blue;
			Console.Write(b);
			Console.ForegroundColor = ConsoleColor.Gray;
			Console.Write("]: ");

			// Then move the cursor over a little bit to a constant spot (this is so things line up and look nice)
			Console.CursorLeft = 28;
			Console.WriteLine("\"{0}\"", string.Concat((char)r, (char)g, (char)b));
		}
	}
}
