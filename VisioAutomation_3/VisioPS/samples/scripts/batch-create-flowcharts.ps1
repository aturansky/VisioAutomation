Write-Host "Num Args:" $args.Length; 
foreach ($arg in $args) 
{  
    Write-Host "Arg: $arg"; 
} 

Function CreateFolder($p)
{
    $parentpath = split-path $p
    $basename = split-path $p -leaf

    AssertFolderExists($parentpath) 
    
    new-item  -path $parentpath -name $basename -type directory -force  | out-null 
    AssertFolderExists($p) 
}

Function AssertFolderExists($p)
{    
    $result = Test-Path -path $p –pathType container
    Write-Host ">>" $p $result
    if ($result -ne $True)
    {
        $msg = "Folder does not exist: " + $p
        throw $msg
    }
    else
    {
    }
}

$outputpath = "D:\outputfolder\"
$inputpath= "D:\saveenr\code\tfs01-codeplex-com\VisioAutomation\VisioAutomation_2_DEV\VisioAutomation\VisioPS\samples\flowcharts"

AssertFolderExists( $outputpath )
AssertFolderExists( $inputpath )

$inputfiles = get-childitem $inputpath

import-module D:\saveenr\code\tfs01-codeplex-com\VisioAutomation\VisioAutomation_2_DEV\VisioAutomation\VisioPS\bin\Debug\VisioPS.Dll

Write-Host "Input files:" $inputfiles

new-VisioApplication
foreach ($inputfile in $inputfiles)
{
    Write-Host 
    Write-Host "Input file:" $inputfile
    
    $input_file_absname = join-path $inputpath $inputfile 
    $output_file_absname = (join-path $outputpath $inputfile) + ".vsd"
    Write-Host "Input file abs:" $input_file_absname 
    Write-Host "Out file abs:" $output_file_absname 
    
    draw-flowchart -filename $input_file_absname -verbose
    save-drawing -filename $output_file_absname -verbose -debug 

    close-drawing
}
