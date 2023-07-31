import Head from 'next/head'
import Image from 'next/image'
import { Inter } from 'next/font/google'
import styles from '@/styles/Home.module.css'
import { GoogleLogin, GoogleOAuthProvider, useGoogleLogin } from '@react-oauth/google'
import axios from 'axios'

const inter = Inter({ subsets: ['latin'] })

export default function Home() {

  const githubLogin = async () =>{
    
  }

  return (
    <div>
      <GoogleOAuthProvider clientId='912497947825-gchgv5bd8q5jkd4dq9snmteaprc2u7tc.apps.googleusercontent.com'>
        
      <h3>OAuth App</h3>
      <GoogleLogin onSuccess={res =>{
        console.log(res);
      }}
      onError={()=>{
        console.log("Error occured");
        
      }}
      useOneTap
      ></GoogleLogin>

      </GoogleOAuthProvider>
      <iframe src='http://localhost:3000/githuboauth'>
    
    </iframe>
    </div>
  )
}