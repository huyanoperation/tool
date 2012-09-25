//huyan
function AnimationScripToXML()
{
   echo("AnimationScripToXML");
   
   if( isObject( $managedDatablockSet ) )
   {
      echo( "have $managedDatablockSet" );
   }
   
   %count = $managedDatablockSet.getCount();

   echo("%count="@%count);   
   
   for( %i = 0; %i <%count; %i++ )
   {
      if( $managedDatablockSet.getObject(%i).isMemberOfClass( "t2dImageMapDatablock" ) && $managedDatablockSet.getObject(%i).imageMode $= "CELL" )
      {
         echo($managedDatablockSet.getObject(%i).getName());
         WriteDataBlockToXML( $managedDatablockSet.getObject(%i) );
      }
   }
}

function WriteDataBlockToXML( %Object )
{
  echo( "WriteDotaBlockToXML" );
  echo( fileBase( %Object.imageName ) @ ".png" );
  
  //%plistName = expandFileName( fileBase( %Object.imageName ) @ ".plist" );
  %plistName = fileBase( %Object.imageName ) @ ".plist";
  
  %path = expandFilename("^game/data/images/");
  
  %xml = new ScriptObject() { class = "XML"; };
   if( %xml.beginWrite( %path@%plistName ) )
   {
      %xml.writeClassBegin( "dict" );
         
         %xml.writeField( "key", "frames" );
         
            %xml.writeClassBegin( "dict" );

               if( %Object.cellCountX != -1 )
               {
                  %XFrameCount = %Object.cellCountX;
               }
               else
               {
                  %XFrameCount = 1;
               }
               
               if( %Object.cellCountY != -1 )
               {
                  %YFrameCount = %Object.cellCountY;
               }
               else
               {
                  %YFrameCount = 1;
               }
               echo("%XFrameCount="@%XFrameCount);
               echo("%YFrameCount="@%YFrameCount);            
            
               %count = 0;
               for( %i = 0; %i < %YFrameCount; %i++ )
               {
                  for( %j = 0; %j < %XFrameCount; %j++ )
                  {
                     %xml.writeField( "key", fileBase( %Object.imageName ) @ %count++ );               
                     %xml.writeClassBegin( "dict" );
                     
                        %xml.writeField( "key", "x" );
                        %xml.writeField("string", %Object.cellWidth * %j);
                        %xml.writeField( "key", "y" );
                        %xml.writeField("string", %Object.cellheight * %i);
                        
                        %xml.writeField( "key", "width" );
                        %xml.writeField("string", %Object.cellWidth );
                         %xml.writeField( "key", "height" );
                        %xml.writeField("string", %Object.cellheight );
                        
                        %xml.writeField( "key", "offsetX" );
                        %xml.writeField("string", %Object.cellOffsetX );
                        %xml.writeField( "key", "offsetY" );
                        %xml.writeField("string", %Object.cellOffsetY );
                        
                        %xml.writeField( "key", "originalWidth" );
                        %xml.writeField("string", %Object.cellWidth );
                        %xml.writeField( "key", "originalHeight" );
                        %xml.writeField("string", %Object.cellheight );
                        
                        
                     %xml.writeClassEnd();
                  }
               }
               
         
         %xml.writeClassEnd();
         
         %xml.writeField( "key", "metadata" );
            %xml.writeClassBegin( "dict" );
                %xml.writeField( "key", "format" );
                %xml.writeField( "integer",0 );
                %xml.writeField( "key","textureFileName" );
                %xml.writeField( "string",fileBase( %Object.imageName ) @ ".png" );
//                %xml.writeField( "key", "NumberOfFramesPerSecond" );
//                for(%x = 0; %x < $managedDatablockSet.getCount(); %x++)
//                {
//                     if( $managedDatablockSet.getObject(%x).imageMap $= %Object.getName() )
//                     {
//                        %xml.writeField( "string",$managedDatablockSet.getObject(%x).animationTime / getWordCount( $managedDatablockSet.getObject(%x).animationFrames ) );
//                        echo( $managedDatablockSet.getObject(%x).imageMap );
//                        echo( "--------------------------------------------------------------------" );                        
//                        break;
//                     }
//                }
                
            %xml.writeClassEnd();        
         
      %xml.writeClassEnd();
         

      
      %xml.endWrite();
   }   
}