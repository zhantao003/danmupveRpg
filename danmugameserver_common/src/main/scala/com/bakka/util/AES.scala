package com.bakka.util

import javax.crypto.{Cipher, SecretKey}
import javax.crypto.spec.{IvParameterSpec, SecretKeySpec}
import org.apache.commons.codec.binary.Base64
import sun.misc.{BASE64Decoder, BASE64Encoder}

import java.security.Security

/**
 * Created by @author yuanjchen on 4/12/18
 */
object AES{
  //加密秘钥
  val key = "xzvke23I9pEYvPo3"
  //加密偏移量
  val iv = "SoFWWNwBUUOJVxNT"

  def AESEncode(content: String): String = {
    val cipher = Cipher.getInstance("AES/CBC/PKCS5Padding")
    val keyspec = new SecretKeySpec(key.getBytes("utf-8"), "AES")
    val ivspec = new IvParameterSpec(iv.getBytes("utf-8"))
    cipher.init(Cipher.ENCRYPT_MODE, keyspec,ivspec)
    Base64.encodeBase64String(cipher.doFinal(content.getBytes("utf-8")))
  }

  def AESDecode(content: String): String = {
    val cipher = Cipher.getInstance("AES/CBC/PKCS5Padding")
    val keyspec = new SecretKeySpec(key.getBytes("utf-8"), "AES")
    val ivspec = new IvParameterSpec(iv.getBytes("utf-8"))
    cipher.init(Cipher.DECRYPT_MODE, keyspec,ivspec)
    new String(cipher.doFinal(new BASE64Decoder().decodeBuffer(content)),"utf-8")
  }

}