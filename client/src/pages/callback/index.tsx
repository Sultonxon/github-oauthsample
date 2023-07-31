import axios from "axios";
import { useRouter } from "next/router";
import { useEffect } from "react";


function GHCallback(props: any) {
    console.log(props);
    const router = useRouter();
    const code = router.query['code'];
    console.log(code);
    useEffect( () =>{
        if(code){
            axios.post('http://localhost:5264/api/auth/github/GitHubAuth', {authToken: code, AuthToken:code}, {

            }).then(res =>{
                console.log('response', res);
                console.log('code', code);
                
            }).catch(e =>{
                console.log(e);
                console.log(code);
                
            });
        }
    })
    return (
        <div>
            <iframe >

            </iframe>
        </div>
    )
}

export default GHCallback;