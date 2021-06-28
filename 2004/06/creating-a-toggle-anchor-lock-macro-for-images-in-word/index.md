
I recently wanted a button or keystroke that could automatically toggle the Lock Anchor state for shapes in Microsoft Word.  Unfortunately, there didn't seem to be a built-in Word action for doing this.  Using a tip from Cindy Meister I created the following macro that does the trick nicely:

> Sub ToggleShapeAnchor() Dim newlockAnchorSetting As Boolean If Selection.Type = wdSelectionShape Then If (Selection.ShapeRange.Count >= 1) Then newlockAnchorSetting = Not Selection.ShapeRange(1).LockAnchor End If For Each Shape In Selection.ShapeRange Shape.LockAnchor = newlockAnchorSetting Next End If End Sub

I also had a problem with trying to make fine adjustments of the shapes.  Each adjustment caused the shape to jump a couple inches up the page.  Further adjustment caused it to jump again.  Cindy informed me that this was generally indicative of damage in the binary structures of the control page layout and advised I tried round tripping the file to RTF, WordML, or HTML.  I also found that turning on and off the anchor sometimes seemed to get particular images positioning correctly again.
