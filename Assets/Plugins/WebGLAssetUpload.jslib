var WebGLAssetUpload = {

    UploadAsset: function(gameObjectNamePtr, uploadUrlPtr, filterPtr, maxBytesPtr, isMultiSelect) {
        const CLOUDINARY_UPLOAD_PRESET = "asset_preset";
        const PROGRESS_CHANGED_METHOD = "UploadProgressChanged";
        const ASSET_UPLOADED_METHOD = "AssetUploaded";
        const UPLOAD_FAILED_METHOD = "UploadFailed";
        const FILE_TOO_BIG_METHOD = "FileTooBig";
        const FOLDER_NAME = 'WormTomb/Gallery';
        
        gameObjectName = Pointer_stringify(gameObjectNamePtr);
        uploadUrl = Pointer_stringify(uploadUrlPtr);
        filter = Pointer_stringify(filterPtr);
        maxBytes = Number(Pointer_stringify(maxBytesPtr));

        // Delete if HTML element already exists.
        var fileInput = document.getElementById(gameObjectName)
        if (fileInput) {
            document.body.removeChild(fileInput);
        }

        fileInput = document.createElement('input');
        fileInput.setAttribute('id', gameObjectName);
        fileInput.setAttribute('type', 'file');
        fileInput.setAttribute('style', 'display:none;');
        fileInput.setAttribute('style', 'visibility:hidden;');

        if (isMultiSelect) {
            fileInput.setAttribute('multiple', '');
        }

        if (filter) {
            fileInput.setAttribute('accept', filter);
        }

        // Use onclick to clear input so onchange can be triggered by the same path selection multiple times.
        fileInput.onclick = function(event) {
            event.target.value = '';
        };

        fileInput.onchange = function(event) {
            
            // Check that all files are within Cloudinary's file size limits.
            for (var i = 0; i < event.target.files.length; i++)
            {
                if (event.target.files[i].size > maxBytes) {
                    SendMessage(gameObjectName, FILE_TOO_BIG_METHOD, event.target.files[i].name);
                    console.log("File size exceeds maximum byte limit: " + event.target.files[i].name);
                    return;
                }
            }

            // Upload each file to Cloudinary.
            for (var i = 0; i < event.target.files.length; i++) {
                var file = event.target.files[i];
                var formData = new FormData();
                formData.append("file", file);
                formData.append("upload_preset", CLOUDINARY_UPLOAD_PRESET);
                formData.append("folder", FOLDER_NAME);
                
                axios.post(uploadUrl, formData, { 
                    // Report upload progress to Unity.
                    onUploadProgress: function(progressEvent) {
                        var progressObject = {loadedKilobytes: progressEvent.loaded, totalKilobytes: progressEvent.total};
                        SendMessage(gameObjectName, PROGRESS_CHANGED_METHOD, JSON.stringify(progressObject)); 
                    }
                })
                // Send URL and file name as JSON to Unity upon a successful upload response.
                .then(function(response) 
                {
                    var responseObject = 
                        {
                            Filename: response.data.original_filename, 
                            Url: response.data.secure_url
                        };
                    SendMessage(gameObjectName, ASSET_UPLOADED_METHOD, JSON.stringify(responseObject));
                })
                // Report upload error to Unity.
                .catch(function(error)
                {
                    console.log(error);
                    SendMessage(gameObjectName, UPLOAD_FAILED_METHOD, error);
                });
            }

            // Remove fileInput element.
            if (fileInput) {
                document.body.removeChild(fileInput);
            }
        };

        document.body.appendChild(fileInput);

        document.onmouseup = function() {
            fileInput.click();
            document.onmouseup = null;
        }
    }
};
mergeInto(LibraryManager.library, WebGLAssetUpload);