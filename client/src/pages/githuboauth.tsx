


function GithubOauth(){
    return (<form method='get' action={'https://github.com/login/oauth/authorize'}>
    <input type='hidden' value={'c48925d14cde0345c9c5'} name='client_id'/>
    <input type='hidden' value={'http://127.0.0.1:3000/callback'} name='redirect_uri'/>
    <input type='hidden' value={'user'} name='scope' />
    <button>Github login</button>  
    </form>)
}

export default GithubOauth;