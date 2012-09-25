//$LevelEditorSaving::LayersNeedingSave
function LevelToXML()
{
   echo("LevelToXML");
   
   %lastWindow = ToolManager.getLastWindow();
   %sceneGraph = %lastWindow.getScenegraph();
   
   %xml = new ScriptObject() { class = "XML"; };

   %path = expandFilename("^game/data/levels/");     
     
   //%plistName = fileBase( %sceneGraph.getName() ) @ ".plist";
   
   %plistName = fileBase($levelEditor::LastLevel) @ ".plist";
   echo("%plistName="@%plistName);
   if( %xml.beginWrite( %path @ %plistName ) )
   {
      %xml.writeClassBegin( "dict" );
         
         %xml.writeField( "key", "images" );
         
            %xml.writeClassBegin( "dict" );
        
                  %count = 0;
                  for( %i = 0; %i < %sceneGraph.getCount(); %i++ )
                  {
                        if( %sceneGraph.getObject(%i).getClassName() $= "t2dStaticSprite" )
                        {
                           if( %sceneGraph.getObject(%i).getName() $= ""  )
                           {
                              %xml.writeField( "key", "key"@%count++ );
                           }
                           else
                           {
                              %xml.writeField( "key", %sceneGraph.getObject(%i).getName() );
                           }
                           %xml.writeClassBegin( "dict" );
                     
                           %xml.writeField( "key", "x" );
                           %xml.writeField("string", getWord( %sceneGraph.getObject(%i).Position,0));
                           %xml.writeField( "key", "y" );
                           %xml.writeField("string", getWord( %sceneGraph.getObject(%i).Position,1));
                        
                           %xml.writeField( "key", "width" );
                           %xml.writeField("string",  getWord( %sceneGraph.getObject(%i).Size,0));
                           %xml.writeField( "key", "height" );
                           %xml.writeField("string", getWord( %sceneGraph.getObject(%i).Size,1) );

                           %xml.writeField( "key", "layer" );
                           %xml.writeField("string", %sceneGraph.getObject(%i).Layer ); 
                           
                           %xml.writeField( "key", "FlipX" );
                           %xml.writeField("string", %sceneGraph.getObject(%i).FlipX );
                           
                           %xml.writeField( "key", "FlipY" );
                           %xml.writeField("string", %sceneGraph.getObject(%i).FlipY );                       
                        
                           %xml.writeField( "key", "image" );
                           %xml.writeField("string", fileBase(%sceneGraph.getObject(%i).imageMap.imageName)@".png" );

                           %xml.writeClassEnd();
                        }
                        else if( %sceneGraph.getObject(%i).getClassName() $= "t2dAnimatedSprite" )
                        {
                           if( %sceneGraph.getObject(%i).getName() $= ""  )
                           {
                              %xml.writeField( "key", "t2dAnimatedSprite_" @ "key"@%count++ );
                           }
                           else
                           {
                              %xml.writeField( "key", "t2dAnimatedSprite_" @ %sceneGraph.getObject(%i).getName() );
                           }
                           %xml.writeClassBegin( "dict" );
                           
                           %xml.writeField( "key", "x" );
                           %xml.writeField("string", getWord( %sceneGraph.getObject(%i).Position,0));
                           %xml.writeField( "key", "y" );
                           %xml.writeField("string", getWord( %sceneGraph.getObject(%i).Position,1));
                        
                           %xml.writeField( "key", "width" );
                           %xml.writeField("string",  getWord( %sceneGraph.getObject(%i).Size,0));
                           %xml.writeField( "key", "height" );
                           %xml.writeField("string", getWord( %sceneGraph.getObject(%i).Size,1) );

                           %xml.writeField( "key", "layer" );
                           %xml.writeField("string", %sceneGraph.getObject(%i).Layer ); 
                           
                           %xml.writeField( "key", "FlipX" );
                           %xml.writeField("string", %sceneGraph.getObject(%i).FlipX );
                           
                           %xml.writeField( "key", "FlipY" );
                           %xml.writeField("string", %sceneGraph.getObject(%i).FlipY ); 
                           
                           %xml.writeField( "key", "plist" );
                           %xml.writeField("string", fileBase(%sceneGraph.getObject(%i).animationName.imageMap.imageName)@".plist" );
                           
                           %xml.writeField( "key", "startframe" );
                           %xml.writeField("string", getWord( %sceneGraph.getObject(%i).animationName.animationFrames, 0 ) );
                           
                           %xml.writeField( "key", "endframe" );
                           %frameCount = getWordCount( %sceneGraph.getObject(%i).animationName.animationFrames );
                           %xml.writeField("string", getWord( %sceneGraph.getObject(%i).animationName.animationFrames, %frameCount - 1 ) );
                           
                           %xml.writeField( "key", "animationtime" );
                           %xml.writeField( "string",%sceneGraph.getObject(%i).animationName.animationTime ); 
                           
                           %xml.writeClassEnd();
                        }
                        else if( %sceneGraph.getObject(%i).getClassName() $= "t2dSceneObject" )
                        {
                           if( %sceneGraph.getObject(%i).getName() $= ""  )
                           {
                              %xml.writeField( "key", "t2dSceneObject_" @ "key" @ %count++ );
                           }
                           else
                           {
                              %xml.writeField( "key", "t2dSceneObject_" @ %sceneGraph.getObject(%i).getName() );
                           }
                           %xml.writeClassBegin( "dict" );
                     
                           %xml.writeField( "key", "x" );
                           %xml.writeField("string", getWord( %sceneGraph.getObject(%i).Position,0));
                           %xml.writeField( "key", "y" );
                           %xml.writeField("string", getWord( %sceneGraph.getObject(%i).Position,1));
                        
                           %xml.writeField( "key", "width" );
                           %xml.writeField("string",  getWord( %sceneGraph.getObject(%i).Size,0));
                           %xml.writeField( "key", "height" );
                           %xml.writeField("string", getWord( %sceneGraph.getObject(%i).Size,1) );
                      
                           %xml.writeClassEnd();
                        }
                     }
                  }
               %xml.writeClassEnd();
         %xml.endWrite();
   }
}