Portafilter
-----

This is a Azure Functions host of BaristaCore.

Caveat: Despite running perfectly locally, when deployed to Azure Functions (as of now) despite
having x64 enabled on the application configuration page, Azure Functions loads dotnet core in x86

wtf.

---
https://github.com/Azure/Azure-Functions/issues/651 to track, but, 
given the issue queue is growing, and since there's no way of knowing...

Round the world and home again
That’s the sailor’s way
Faster faster, faster faster

There’s no earthly way of knowing
Which direction we are going
There’s no knowing where we’re rowing
Or which way the river’s flowing

Is it raining, is it snowing
Is a hurricane a-blowing

Not a speck of light is showing
So the danger must be growing
Are the fires of Hell a-glowing
Is the grisly reaper mowing

Yes, the danger must be growing
For the rowers keep on rowing
And they’re certainly not showing
Any signs that they are slowing

– ‘Wondrous Boat Ride’, from Willy Wonka and the Chocolate Factory (1971)


---
AWS Lambda doesn't support dotnet core 2.0 yet. So we're stuck for now.