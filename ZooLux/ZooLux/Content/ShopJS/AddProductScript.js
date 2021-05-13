<<<<<<< HEAD
﻿// Script to preview the images
$(function() {
=======
﻿$(function () {
>>>>>>> b83509fab7edc5605acb5de78e5ed69d2cdcd23a

    /* Preview selected image */
    function readUrl(input) {
        if (input.files && input.files[0]) {
            var reader = new FileReader();

            reader.onload = function(e) {
                $("img#imgpreview")
                    .attr("src", e.target.result)
                    .width(200)
                    .height(200);
            }

            reader.readAsDataURL(input.files[0]);
        }
    }

    // Function, after page reloading for output the image
    $("#imageUpload").change(function() {
        readUrl(this);
    });
<<<<<<< HEAD

    /*-----------------------------------------------------------*/
=======
>>>>>>> b83509fab7edc5605acb5de78e5ed69d2cdcd23a
});