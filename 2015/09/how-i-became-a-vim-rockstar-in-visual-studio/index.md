

# A Brief History Lesson

As a software developer, we spend a lot of our time using text editors. Most text editors follow the Emacs standard that [harkens all the way back to the 1970s](https://en.wikipedia.org/wiki/Emacs#History) and the venerable [PDP-10](https://en.wikipedia.org/wiki/PDP-10). Emacs’ WYSIWYG style of editing is best suited to a stream-of-consciousness and linear style of editing. The focus is, foremost, on being able to type all the letters and punctuation needed to capture your thoughts. Cursor movement, block selection and copy/paste all rely on either moving your hands off the home row or using modifier [keys like CTRL and ALT](https://www.catb.org/jargon/html/Q/quadruple-bucky.html). After all, the [keyboards](https://www.catb.org/jargon/html/S/space-cadet-keyboard.html) on the MIT LISP machines that Emacs was designed for allowed the user to type an astonishing 8000 different characters.

[![Screen Shot 2015-09-23 at 3.17.38 PM](https://intellitect.com/wp-content/uploads/2015/09/Screen-Shot-2015-09-23-at-3.17.38-PM.png)](https://intellitect.com/wp-content/uploads/2015/09/Screen-Shot-2015-09-23-at-3.17.38-PM.png "How I Became A Vim Rockstar In Visual Studio")

Vim is a contraction of Vi IMproved, as it improves upon the visual mode of a screen-oriented text editor called **vi**, [written by Bill Joy in 1976](https://en.wikipedia.org/wiki/Vi), that first showed up in [BSD](https://en.wikipedia.org/wiki/Berkeley_Software_Distribution).  **vi** itself was a derivative of the visual mode of an older line editor called **ex** that Bill Joy had written with Chuck Haley while attending Berkeley. The economy of keys on the Lear Siegler ADM-3A terminal that Joy used greatly influenced the commands that **ex** and **vi** used in their operation. [First released in 1991 by Bram Moolenaar](https://en.wikipedia.org/wiki/Vim_(text_editor)#History) for the Amiga, Vim’s customization and portability have been widely adopted in the Linux community. Vi/Vim greatly emphasizes text _editing_ over creation and economy of movement over modifier keys or using the mouse. As software developers, you will quickly recognize that most of the work we do with source code files is editing or changing of text; not prosaic writing. Considering the number of keystrokes you have left in your hands, why not use them more efficiently?

# Learning Vim

Despite its terse syntax, Vim is all about being easy to learn. On Linux and OSX it comes with “vimtutor”, a command-line tutorial that takes a new user all the way through Vim’s functionality. “vimtutor” is also available in the Git Bash shell if you have installed Git on Windows. For users with a sense of whimsy, a more fun alternative exists at [https://vim-adventures.com/](https://vim-adventures.com/). The free version will take you through all the movement keys, and for $25 you can get a 6-month license that will take you through everything else.

I would also recommend printing out one of the many Vim cheat sheets available online. Some favorites of mine include [this programmer-centric one](https://michael.peopleofhonoronly.com/vim/), and a [simple SVG one](https://www.viemu.com/a_vi_vim_graphical_cheat_sheet_tutorial.html). Print and attach to your cube wall for some added reputation from your local \*nix beards.

Once you have spent a little time learning the basics in a tutorial fashion, it’s time to install the [VsVim extension](https://visualstudiogallery.msdn.microsoft.com/59ca71b3-a4a3-46ca-8fe1-0e90e3f79329) into Visual Studio. Buckle-up, buttercup, the next few days are going to be a little rough. For inspiration, watch Ian Davis as he cruises through the Gilded Rose Kata with VsVim.

# Stick With It!

For an initial foray into learning Vim within Visual Studio, pick an easy project to work on-- some routine maintenance or a spike or demo. I won’t lie-- your productivity will suffer horribly for a few days, and it will feel like your hands are no longer listening to your brain. For me, it was a good four or five days until I felt confident enough to tackle my normal workload.

Remember, VsVim provides an escape hatch: “Ctrl-Shift-F12”, if you get yourself really bound around the axle.

# [![Screen Shot 2015-09-23 at 3.18.06 PM](https://intellitect.com/wp-content/uploads/2015/09/Screen-Shot-2015-09-23-at-3.18.06-PM.png)](https://intellitect.com/wp-content/uploads/2015/09/Screen-Shot-2015-09-23-at-3.18.06-PM.png "How I Became A Vim Rockstar In Visual Studio")

During this steep part of the learning curve, try to set small, easily accomplished, goals for yourself and then celebrate when you meet them. Internal positive reinforcement here will yield huge dividends later as you start to feel yourself surpassing your previous, non-Vim, productivity.

# The Escape Key

At this point, you are probably wondering why keyboard designers put the Esc key way up in the north 40, as it is used so much in **vi**. Remember that the keyboard the **vi** creator was using was on a [Lear Siegler ADM-3A](https://en.wikipedia.org/wiki/ADM-3A#/media/File:KB_Terminal_ADM3A.svg) terminal, and the Esc key was where the Tab key is on what we consider a modern keyboard. Fortunately, Windows makes it relatively easy to remap keyboard keys. Fire up a command prompt and use [Chocolatey](https://chocolatey.org/) to download [SharpKeys](https://sharpkeys.codeplex.com/):

```powershell
C:\\> choco install sharpkeys
```

Use SharpKeys to remap the Caps Lock key to be a second Escape key, and your carpal tunnels will thank you. As an added benefit, you will no longer be able to program in COBOL or accidentally [send shouty emails](https://theoatmeal.com/pl/minor_differences/capslock) to your colleagues.

# Intermediate Beginner Code-Fu

In this video, I use V  to select the line in visual mode, then }  to jump to the next blank line, d  to cut,{  to jump above the current method, and p  to paste.

[embed]https://youtu.be/Ldr4hUA1fYg[/embed]

Granted, ReSharper gives you the same feature with “Alt+Up/Down Arrow” to jump between members, and “Ctrl+Shift+Alt+Up/Down Arrow” to move members around-- [for a price](https://www.jetbrains.com/resharper/buy/). Using the i  (for Inner) selector adds a whole new level of awesomeness to the Change mode.

[embed]https://youtu.be/F5mj7RNhmcQ[/embed]

You can also flex your RegEx prowess by using the native regular expression based search and replace. The Substitute command allows you to find and replace bits of text or whole lines either in the current line or globally throughout the file, and accepts RegEx patterns by default.

[embed]https://youtu.be/0fXQe-Qm8rU[/embed]

# When You Can Snatch The Pebble From Master Po

[![Screen Shot 2015-09-23 at 3.18.32 PM](https://intellitect.com/wp-content/uploads/2015/09/Screen-Shot-2015-09-23-at-3.18.32-PM.png)](https://intellitect.com/wp-content/uploads/2015/09/Screen-Shot-2015-09-23-at-3.18.32-PM.png "How I Became A Vim Rockstar In Visual Studio")

In addition to the normal rotating clipboard built into Visual Studio, VIM provides its own storage location for bits of text you yank or d elete called registers. Type :reg  to get a list of everything currently stored in the registers. There are numbered registers that function like the rotating clipboard, and these line up with the numeric row on the keyboard. To paste or manipulate with what’s stored in a register, just use “  and the numeric identifier before your command. For example, “1p  pastes the contents of register 1 after the current position. Additionally, all the other keys on the keyboard can be used as named registers (and won’t be overwritten as more things get added to the clipboard). This is useful if you want to stash a bit of oft-used code, but don’t want to make a snippet. For example, “aY  will yank the entire line into register ‘a’. Being able to push and pop bits of code into the editor with nothing more than the keyboard is tremendously useful when coding.

Similar to registers, Vim allows you to store “marks” or specific locations inside files. These can be used as insert points for pasting or just navigation. I realize Visual Studio already has the concept of bookmarks, but it involves a Vulcan-nerve-pinch key sequence (Ctrl+Shift+V). To mark a location, simply press m followed by any key on the keyboard. Type :marks  to view a list of all your saved marks. Using lower-case letters for marks scopes them to the particular file you are in, and using upper-case letters creates global marks that will change your focus to the file they are in. Use ‘  and the mark name to jump to that location.

[embed]https://youtu.be/Zr9Sj1d1pyI[/embed]

# Macro-VIM-otic Cooking With Gas

Once you feel you have mastered most of the utility in Vim, you can begin to meta-program using Macros. This feature allows you to record and playback interesting sequences of Vim commands to do more complicated transformations that aren’t possible with RegEx substitutions. You can even store them as [mappings](https://vim.wikia.com/wiki/Mapping_keys_in_Vim_-_Tutorial_(Part_1)) in your .vimrc configuration file and make them available to yourself permanently in Visual Studio. To begin recording, use q  followed by a register name (macros are stored in registers, so be careful you don’t overwrite some code you have saved in one). Press q  again when you have recorded your extra-tricky text manipulation. Move the cursor to where you want to execute the macro again, and then use @  and the register name. Like most commands, you can prefix with a number to have your macro executed a number of times.

[embed]https://youtu.be/5o9ueMQxUBk[/embed]

# Next Steps…

At this point, you can continue to refine your Vim command skillset, (I recommend [https://www.vimgolf.com/](https://www.vimgolf.com/) for the truly radical keystroke conservationist), or even consider a non-QWERTY keyboard layout such as Colemak or Workman. Treat yourself to a [nice clicky-clacky keyboard](https://www.wasdkeyboards.com/index.php/products/code-keyboard.html) so that everyone around you will know how efficient you have become. Or add some [foot pedals](https://github.com/alevchuk/vim-clutch) for Esc and the modifier keys (Shift, Ctrl, Alt). Regardless of where your path takes you, you will get many more hours of coding out of the finite amount of keystrokes left in your hands.
