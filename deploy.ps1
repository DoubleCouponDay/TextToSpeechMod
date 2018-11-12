#INSTRUCTIONS
#1. place in the root directory of the text to speech mod project
#2. open a command shell
#3. change the shell's directory to the project's root directory, using a "cd" command.
#4. enter the name of this script in the shell.
#5. follow the prompts.
#6. profit!

$outputdir = Read-Host -Prompt "Enter an absolute path to the Space Engineers mod directory: "
$inputdir = ".\mirrored workshop build"
$outputfolder = "texttospeechmod"
$projectfile = "SE TextToSpeechMod.csproj"
$forwardslashchar = "/"

Write-Host "deploying text to speech mod..."

if ($outputdir.EndsWith("/"))
{
    $outputfolder = $outputdir + $outputfolder + $forwardslashchar
}

else
{
    $outputfolder = $outputdir + $forwardslashchar + $outputfolder + $forwardslashchar
}
Write-Host "using output folder: $outputfolder"
$scriptdir = $outputdirScripts + $forwardslashchar + "folder required here" + $forwardslashchar

Write-Host "building the project..."
"MSBuild.exe " + $projectfile

if ($LASTEXITCODE -ne 0)
{
    Write-Host "build failed! get it compiling in visual studio before continuing."
    Exit-PSHostProcess
}
Write-Host "build succeeded."

Write-Host "copying mirrored directory..."
Copy-Item $inputdir -Destination $outputdir -Recurse
Write-Host "mirror directory copied."

Write-Host "overwriting with fresh script files..."
$getscriptsliteral = "Get-ChildItem -Path '**\*.cs' -Recurse -Exclude @('.\mirrored workshop build', '.\.git', '.\bin', '.\.vs', '.\vscode', '.\obj', '.\Properties')"
$scriptfilesaddresses = Invoke-Expression $getscriptsliteral
Write-Host "script files that will be deployed:"
Write-Host Invoke-Expression $getscriptsliteral " -Name"
Copy-Item -Path $scriptfilesaddresses -Destination $scriptdir -Force
Write-Host "mod deployed."